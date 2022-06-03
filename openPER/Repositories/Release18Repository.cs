using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable StringLiteralTypo

namespace openPER.Repositories
{
    public class Release18Repository : IRepository
    {
        IConfiguration _config;
        private readonly string _pathToDb;
        public Release18Repository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 18);
            if (release != null)
            {
                _pathToDb = System.IO.Path.Combine(release.FolderName, release.DbName);
            }


        }

        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode)
        {
            var t = new TableModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            t.CatalogueCode = catalogueCode;
            t.GroupCode = groupCode;    
            t.SubGroupCode= subGroupCode;
            t.SubSubGroupCode = sgsCode;
            //            t.MakeDesc = GetMakeDescription(makeCode, connection);
            //          t.ModelDesc = GetModelDescription(makeCode, modelCode, connection);
            //        t.CatalogueDesc = GetCatalogueDescription(makeCode, modelCode, catalogueCode, connection);
            t.GroupDesc = GetGroupDescription(groupCode, languageCode, connection);
            t.SubGroupDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
            // TODO Add variant information to sgs description
            t.SgsDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
            t.Parts = GetTableParts(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode, connection);
            t.DrawingNumbers = GetDrawingNumbers(catalogueCode, groupCode, subGroupCode, sgsCode, connection);
            t.Narratives = GetSgsNarrative(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
            t.CurrentDrawing = drawingNumber;
            return t;
        }
        public List<MakeModel> GetAllMakes()
        {
            var rc = new List<MakeModel>
            {
                new ("F", "Fiat"),
                new ("T", "Fiat commercial"),
                new ("L", "Lancia"),
                new ("R", "Alfa Romeo")
            };
            //using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            //var sql = @"SELECT MK_COD, MK_DSC FROM MAKES ORDER BY MK_DSC ";
            //connection.RunSqlAllRows(sql, (reader) =>
            //{
            //    var m = new MakeModel
            //    {
            //        Code = reader.GetString(0),
            //        Description = reader.GetString(1)
            //    };
            //    rc.Add(m);

            //}, null);
            return rc;

        }

        private List<int> GetDrawingNumbers(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, SqliteConnection connection)
        {
            var rc = new List<int>();
            var sql = @"SELECT DISTINCT DRW_NUM 
                            FROM TBDATA  
                            WHERE SGS_COD = $p1 AND SGRP_COD = $p2 AND GRP_COD = $p3 AND CAT_COD = $p4 ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                rc.Add(reader.GetInt32(0));
            }, sgsCode, subGroupCode, groupCode, catalogueCode);
            return rc;
        }

        private List<TablePartModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode, SqliteConnection connection)
        {
            var parts = new List<TablePartModel>();
            var sql = @"SELECT TBD_RIF, PRT_COD, TBD_QTY, CDS_DSC, TBD_NOTE1, TBD_NOTE2, TBD_NOTE3,
                                        TBD_SEQ, NTS_DSC, TBD_VAL_FORMULA, TBD_AGG_DSC
                                        FROM TBDATA
                                        JOIN CODES_DSC ON TBDATA.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $p1
                                        LEFT OUTER JOIN NOTES_DSC ON NOTES_DSC.NTS_COD = TBDATA.NTS_COD AND NOTES_DSC.LNG_COD = $p1
                                        WHERE DRW_NUM = $p2 AND SGS_COD = $p3 AND SGRP_COD = $p4 AND GRP_COD = $p5 AND CAT_COD = $p6 
                                        ORDER BY TBD_RIF,TBD_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var part = new TablePartModel
                {
                    PartNumber = reader.GetDouble(1),
                    TableOrder = reader.GetInt32(0),
                    Quantity = reader.GetString(2),
                    Description = reader.GetString(3),
                    Notes1 = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Notes2 = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Notes3 = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Sequence = reader.GetString(7),
                    Notes = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    Compatibility = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    FurtherDescription = reader.IsDBNull(10) ? "" : reader.GetString(10)
                };
                parts.Add(part);

            }, languageCode, drawingNumber, sgsCode, subGroupCode, groupCode, catalogueCode);

            foreach (var part in parts)
            {
                part.Modifications = GetPartModifications(part, catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode, connection);
            }
            return parts;

        }
        private List<ModificationModel> GetPartModifications(TablePartModel part, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode, SqliteConnection connection)
        {
            var modifications = new List<ModificationModel>();
            var sql = @"SELECT TBDM_CD, TBDATA_MOD.MDF_COD, TBDM_PROG , MDF_DSC
                                        FROM TBDATA_MOD
                                        JOIN MODIF_DSC ON TBDATA_MOD.MDF_COD = MODIF_DSC.MDF_COD AND MODIF_DSC.LNG_COD = $p1
                                        WHERE DRW_NUM = $p2 AND SGS_COD = $p3 AND SGRP_COD = $p4 AND GRP_COD = $p5 AND CAT_COD = $p6 
                                            AND TBD_RIF = $p7 AND TBD_SEQ = $p8
                                        ORDER BY TBDM_PROG";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new ModificationModel
                {
                    Type = reader.GetString(0),
                    Code = reader.GetInt32(1),
                    Progression = reader.GetInt32(2),
                    Description = reader.GetString(3)
                };
                modifications.Add(mod);

            }, languageCode, drawingNumber, sgsCode, subGroupCode, groupCode, catalogueCode, part.TableOrder, part.Sequence);
            // Get activations for modifications
            foreach (var mod in modifications)
            {
                mod.Activations = GetActivationsForModification(catalogueCode, mod.Code, languageCode, connection);
            }
            return modifications;
        }
        private List<ActivationModel> GetActivationsForModification(string catalogueCode, int modCode, string languageCode, SqliteConnection connection)
        {
            var modifications = new List<ActivationModel>();
            var sql = @"SELECT IFNULL(A.ACT_MASK, ''),IFNULL(M.MDFACT_SPEC, ''), IFNULL(M.ACT_COD, ''), IFNULL(O.OPTK_TYPE, ''), IFNULL(O.OPTK_COD, ''), IFNULL(O.OPTK_DSC, ''),
                    IFNULL(V.VMK_TYPE, ''), IFNULL(V.VMK_COD, ''), IFNULL(V.VMK_DSC, '')
                    FROM MDF_ACT M
                    LEFT OUTER JOIN ACTIVATIONS A ON A.ACT_COD = M.ACT_COD
                    LEFT OUTER JOIN VMK_DSC V ON V.CAT_COD = M.CAT_COD AND V.VMK_TYPE = M.VMK_TYPE AND V.VMK_COD = M.VMK_COD AND V.LNG_COD = $p2
                    LEFT OUTER JOIN OPTKEYS_DSC O ON O.OPTK_TYPE = M.OPTK_TYPE AND O.OPTK_COD = M.OPTK_COD AND O.LNG_COD = $p2
                    WHERE M.CAT_COD = $p3 AND M.MDF_COD = $p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new ActivationModel
                {
                    ActivationDescription = reader.GetString(0) + " " + reader.GetString(1),
                    ActivationCode = reader.GetString(2),
                    OptionType = reader.GetString(3),
                    OptionCode = reader.GetString(4),
                    OptionDescription = reader.GetString(5),
                    VariationType = reader.GetString(6),
                    VariationCode = reader.GetString(7),
                    VariationDescription = reader.GetString(8)
                };
                modifications.Add(mod);

            }, modCode, languageCode, catalogueCode);
            return modifications;
        }
        private string GetCatalogueDescription(string makeCode, string modelCode, string catalogueCode, SqliteConnection connection)
        {
            string rc = "";

            var sql = @"SELECT CMD_DSC FROM COMM_MODELS WHERE MOD_COD = $p1 AND CAT_COD = $p2 ";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, modelCode, catalogueCode);

            return rc;
        }

        private string GetGroupDescription(int groupCode, string languageCode, SqliteConnection connection)
        {
            string rc = "";
            var sql = @"SELECT GRP_DSC FROM GROUPS_DSC WHERE GRP_COD = $p1 AND LNG_COD = $p2 ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, languageCode);
            return rc;
        }

        private string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT SGRP_DSC FROM SUBGROUPS_DSC WHERE SGRP_COD = $p2 AND GRP_COD = $p1 AND LNG_COD = $p3 ";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, subGroupCode, languageCode);
            return rc;
        }


        private string GetMakeDescription(string makeCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT MK_DSC FROM MAKES WHERE MK_COD = $p1";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode);
            return rc;
        }
        private string GetModelDescription(string makeCode, string modelCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT MOD_DSC FROM MODELS WHERE MK_COD = $p1 AND MOD_COD = $p2";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode, modelCode);
            return rc;
        }
        public List<ModelModel> GetAllModels()
        {
            var rc = new List<ModelModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT MOD_COD, MOD_DSC, MK_COD FROM MODELS ORDER BY MOD_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new ModelModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = reader.GetString(2)
                };
                rc.Add(m);
            });
            return rc;

        }
        public List<ModelModel> GetAllModelsForMake(string makeCode)
        {
            var rc = new List<ModelModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT CMG_COD, CMG_DSC FROM COMM_MODGRP WHERE MK2_COD = $p1  ORDER BY CMG_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                var m = new ModelModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = makeCode
                };
                rc.Add(m);
            }, makeCode);
            return rc;
        }

        public List<CatalogueModel> GetAllCatalogues(string makeCode, string modelCode, string languageCode)
        {
            var rc = new List<CatalogueModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT CAT_COD, CAT_DSC FROM CATALOGUES WHERE MK2_COD = $p1 AND CMG_COD = $p2  ORDER BY CAT_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new CatalogueModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = makeCode,
                    ModelCode = modelCode
                };
                rc.Add(m);
            }, makeCode, modelCode);
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var rc = new List<GroupModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select distinct T.GRP_COD, GRP_DSC FROM TBDATA T
                            JOIN GROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.LNG_COD = $p2
                            WHERE CAT_COD = $p1
                            order by T.GRP_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new GroupModel
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1)
                };
                rc.Add(m);
            }, catalogueCode, languageCode);
            //foreach (var group in rc)
            //{
            //    group.SubSubGroups = GetSubgroupsForCatalogueGroup(catalogueCode, group.Code, languageCode);
            //}
            return rc;
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var rc = new List<SubGroupModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select distinct T.SGRP_COD, SGRP_DSC FROM TBDATA T
                            JOIN SUBGROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.SGRP_COD = T.SGRP_COD AND G.LNG_COD = $p1
                            WHERE CAT_COD = $p2 AND T.GRP_COD = $p3
                            order by T.SGRP_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubGroupModel
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    GroupCode = groupCode
                };
                rc.Add(m);
            }, languageCode, catalogueCode, groupCode);
            //foreach (var item in rc)
            //{
            //    item.SubSubGroups = GetSubSubGroupsForCatalogueGroup(catalogueCode, groupCode, item.Code, languageCode);
            //}
            return rc;
        }
        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var rc = new List<SubSubGroupModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select distinct T.SGS_COD FROM TBDATA T
                            WHERE CAT_COD = $p1 AND T.GRP_COD = $p2 AND T.SGRP_COD = $p3
                            order by T.SGS_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubSubGroupModel
                {
                    Code = reader.GetInt32(0),

                };
                m.Narrative = GetSgsNarrative(catalogueCode, groupCode, subGroupCode, m.Code, languageCode);
                rc.Add(m);
            }, catalogueCode, groupCode, subGroupCode);
            return rc;
        }
        private List<string> GetSgsNarrative(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode)
        {
            var rc = new List<string>();
            var sql = @"select V.VMK_TYPE,V.VMK_COD,VMK_DSC from SGS_VAL S
                        JOIN VMK_DSC V ON S.VMK_TYPE = V.VMK_TYPE AND S.VMK_COD = V.VMK_COD AND S.CAT_COD = V.CAT_COD
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);
            sql = @"select SGSMOD_CD,S.MDF_COD,MDF_DSC from SGS_MOD S
                        JOIN MODIF_DSC M ON M.MDF_COD = S.MDF_COD 
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);
            sql = @"select O.OPTK_TYPE,O.OPTK_COD,OPTK_DSC from SGS_OPT S
                        JOIN OPTKEYS_DSC O ON O.OPTK_TYPE = S.OPTK_TYPE AND O.OPTK_COD = S.OPTK_COD 
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);

            return rc;
        }
        private void AddSgsNarratives(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, string sql, List<string> narratives)
        {
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            connection.RunSqlAllRows(sql, (reader) =>
            {
                narratives.Add(reader.GetString(0) + reader.GetString(1) + " " + reader.GetString(2));
            }, catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
        }

        public MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            var m = new MvsModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select M.MOD_COD, M.MVS_VERSION, M.MVS_SERIE, MVS_DSC, MVS_SINCOM_VERS,MVS_ENGINE_TYPE, MV.VMK_DSC, VV.VMK_DSC,
                        M.VMK_TYPE_M||VMK_COD_M, M.VMK_TYPE_V||VMK_COD_V,
                        C.CAT_COD, C.CMD_DSC, COL.DSC_COLORE_INT_VET, MM.MOD_DSC from MVS M 
                        JOIN COMM_MODELS C ON C.MOD_COD = M.MOD_COD
                        JOIN MODELS MM ON MM.MOD_COD = M.MOD_COD
						LEFT OUTER JOIN VMK_DSC MV ON MV.CAT_COD = M.CAT_COD AND MV.VMK_TYPE = M.VMK_TYPE_M AND MV.VMK_COD = M.VMK_COD_M AND MV.LNG_COD = $p1
						LEFT OUTER JOIN VMK_DSC VV ON VV.CAT_COD = M.CAT_COD AND VV.VMK_TYPE = M.VMK_TYPE_V AND VV.VMK_COD = M.VMK_COD_V AND VV.LNG_COD = $p1
                        LEFT OUTER JOIN INTERNAL_COLOURS_DSC COL ON COL.CAT_COD = C.CAT_COD AND COD_COLORE_INT_VET = $p5
					where M.MOD_COD = $p2 AND MVS_VERSION = $p3 AND MVS_SERIE = $p4";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                m.MvsCode = reader.GetString(0);
                m.MvsVersion = reader.GetString(1);
                m.MvsSeries = reader.GetString(2);
                m.Description = reader.GetString(3);
                m.SincomVersion = reader.GetString(4);
                m.EngineType = reader.GetString(5);
                m.EngineDescription = reader.GetString(6);
                m.VariantDescription = reader.GetString(7);
                m.EngineCode = reader.GetString(8);
                m.VariantCode = reader.GetString(9);
                m.CatalogueCode = reader.GetString(10);
                m.CatalogueDescription = reader.GetString(11);
                m.ColourCode = colourCode;
                m.ColourDescription = reader.GetString(12);
                m.ModelDescription = reader.GetString(13);
            }, languageCode, mvsCode, mvsVersion, mvsSeries, colourCode);
            return m;
        }

        public List<LanguageModel> GetAllLanguages()
        {
            var languages = new List<LanguageModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT LNG_COD, LNG_DSC FROM LANG ORDER BY LNG_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new LanguageModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1)
                };
                languages.Add(language);
            });
            return languages;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, DRW_NUM 
                            FROM TBDATA
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    DrawingNumber = reader.GetInt32(4)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode, subSubGroupCode);

            return drawings;
        }

        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, DRW_NUM 
                            FROM TBDATA
                            WHERE CAT_COD = $p1
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    DrawingNumber = reader.GetInt32(4)
                };
                drawings.Add(language);
            }, catalogueCode);
            return drawings;
        }
        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, DRW_NUM 
                            FROM TBDATA
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    DrawingNumber = reader.GetInt32(4)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode);
            return drawings;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, DRW_NUM 
                            FROM TBDATA
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    DrawingNumber = reader.GetInt32(4)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode);
            return drawings;
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            PartModel p = null;
            using (var connection = new SqliteConnection($"Data Source={_pathToDb}"))
            {
                var sql = @"select P.PRT_COD, C.CDS_COD, C.CDS_DSC,F.FAM_COD, F.FAM_DSC, U.UM_COD, U.UM_DSC, PRT_WEIGHT  from PARTS P 
                                JOIN CODES_DSC C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = $p2
                                JOIN FAM_DSC F ON F.FAM_COD = P.PRT_FAM_COD AND F.LNG_COD = $p2
                                LEFT OUTER  JOIN UN_OF_MEAS U ON U.UM_COD = P.UM_COD
                                LEFT OUTER JOIN RPLNT R ON R.RPL_COD = P.PRT_COD
                                where P.PRT_COD = $p1";
                connection.RunSqlFirstRowOnly(sql, (reader) =>
                {
                    p = new PartModel
                    {
                        PartNumber = reader.GetDouble(0),
                        Description = reader.GetString(2),
                        FamilyCode = reader.GetString(3),
                        FamilyDescription = reader.GetString(4),
                        UnitOfSale = reader.GetString(5),
                        Weight = reader.GetInt32(6)
                    };
                }, partNumberSearch, languageCode);

            }
            if (p != null)
            {
                p.Drawings = GetDrawingsForPartNumber(p.PartNumber, languageCode);
            }
            return p;
        }

        private List<PartDrawing> GetDrawingsForPartNumber(double partNumber, string languageCode)
        {
            var drawings = new List<PartDrawing>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT T.CAT_COD,CAT_DSC, T.GRP_COD, T.SGRP_COD, SGS_COD, DRW_NUM, SGRP_DSC,
                                        MK_COD, CMG_COD
                                FROM APPLICABILITY A
                                JOIN TBDATA T ON T.PRT_COD = A.PRT_COD AND T.GRP_COD = A.GRP_COD AND T.SGRP_COD = A.SGRP_COD
                                JOIN SUBGROUPS_DSC SD ON SD.SGRP_COD = T.SGRP_COD AND SD.GRP_COD = T.GRP_COD AND SD.LNG_COD = $p2
                                JOIN CATALOGUES C ON C.CAT_COD = T.CAT_COD
                                WHERE A.PRT_COD = $p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    DrawingNumber = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6)
                };
                drawings.Add(p);
            }, partNumber, languageCode);

            return drawings;
        }

    }
}
