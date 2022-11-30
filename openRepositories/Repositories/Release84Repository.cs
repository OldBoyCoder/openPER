using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

// ReSharper disable StringLiteralTypo

namespace openPERRepositories.Repositories
{
    public class Release84Repository : IRepository
    {
        internal IConfiguration _config;
        internal string _pathToDb;
        public Release84Repository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 84);
            if (release != null)
            {
                _pathToDb = System.IO.Path.Combine(release.FolderName, release.DbName);
            }
        }

        public  MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode)
        {
            var model = new MapImageModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT MAP_NAME, IMG_NAME
                            FROM CATALOGUES
                            WHERE MK_COD = $p1 AND MK2_COD = $p2 AND CAT_COD = $p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                model.MapName = (reader.IsDBNull(0)) ? "" : reader.GetString(0);
                model.ImageName = reader.GetString(1);
            }, makeCode, subMakeCode, catalogueCode);
            return model;
        }
        public  List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
        {
            var map = new List<GroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select MPG_TX, MPG_TY, MPG_INDEX, M.GRP_COD, GRP_DSC from CATALOGUES C
                        JOIN MAP_GRP M ON M.MAP_NAME = C.MAP_NAME
                        JOIN MAP_INFO MI ON MI.MAP_NAME = C.MAP_NAME
                        JOIN GROUPS_DSC GD ON GD.GRP_COD = M.GRP_COD AND LNG_COD = $p2
                        WHERE C.CAT_COD = $p1
                            AND M.GRP_COD IN (SELECT DISTINCT GRP_COD FROM TBDATA WHERE CAT_COD = $p1)
                        ORDER BY MPG_TY, MPG_TX, MPG_INDEX";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new GroupImageMapEntryModel
                {
                    X = reader.GetInt32(0),
                    Y = reader.GetInt32(1),
                    Index = reader.GetInt32(2),
                    GroupCode = reader.GetInt32(3),
                    Description = reader.GetString(4)
                };
                map.Add(m);
            }, catalogueCode, languageCode);
            return map;

        }

        public  MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
        {
            var map = new MapImageModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT MAP_SGRP
                            FROM MAP_VET
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                map.MapName = reader.GetString(0);
            }, catalogue, group);
            if (string.IsNullOrEmpty(map.MapName))
            {
                sql = @"SELECT DISTINCT MAP_NAME
                        FROM MAP_SGRP
                        WHERE GRP_COD = $p1";
                connection.RunSqlFirstRowOnly(sql, (reader) => { map.MapName = reader.GetString(0); }, group);
            }
            sql = @"SELECT DISTINCT IMG_NAME
                        FROM GROUPS
                        WHERE GRP_COD = $p1 AND CAT_COD = $p2";
            connection.RunSqlFirstRowOnly(sql, (reader) => { map.ImageName = reader.GetString(0); }, group, catalogue);

            return map;
        }

        public  List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var map = new List<SubGroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            // sloppy should pass this down
            var mapDetails = GetMapForCatalogueGroup(null, null, null, catalogueCode, groupCode);
            var sql = @"select POINT_X, POINT_Y, M.SGRP_COD, SGRP_DSC from MAP_SGRP M
                        JOIN SUBGROUPS_DSC S ON S.GRP_COD = $p1 AND S.SGRP_COD = M.SGRP_COD AND S.LNG_COD = $p4
                        WHERE M.GRP_COD = $p1 AND MAP_NAME = $p2 AND M.SGRP_COD IN (
                            select distinct T.SGRP_COD FROM TBDATA T
                            WHERE CAT_COD = $p3 AND T.GRP_COD = $p1)";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubGroupImageMapEntryModel
                {
                    X = reader.GetInt32(0),
                    Y = reader.GetInt32(1),
                    GroupCode = groupCode,
                    SubGroupCode = reader.GetInt32(2),
                    Description = reader.GetString(3)
                };
                map.Add(m);
            }, groupCode, mapDetails.MapName, catalogueCode, languageCode);
            return map;
        }

        public  string GetImageNameForDrawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup,
            int drawing, int revision)
        {
            string rc = "";
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT IMG_PATH
                            FROM DRAWINGS
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD  = $p3 AND SGS_COD = $p4 AND VARIANTE = $p5 AND REVISIONE = $p6";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catalogue, group, subgroup, subSubGroup, drawing, revision);
            return rc;
        }

        public  List<MakeModel> GetAllMakes()
        {
            var rc = new List<MakeModel>
            {
                new ("F","F", "FIAT"),
                new ("F","T", "FIAT COMMERCIAL"),
                new ("L","L", "LANCIA"),
                new ("R","R", "ALFA ROMEO"),
                new ("F", "C", "ABARTH")
            };
            return rc;

        }

        public static string GetMakeDescription(string makeCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT MK_DSC FROM MAKES WHERE MK_COD = $p1";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode);
            return rc;
        }

        public  List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var rc = new List<SubSubGroupModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select distinct T.SGS_COD, TD.DSC FROM DRAWINGS T
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p4 AND TD.COD = T.TABLE_DSC_COD
                            WHERE CAT_COD = $p1 AND T.GRP_COD = $p2 AND T.SGRP_COD = $p3
                            order by T.SGS_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubSubGroupModel
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1)

                };
                //m.Modifications = AddSgsModifications(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                // m.Options = AddSgsOptions(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                // m.Variations = AddSgsVariations(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                rc.Add(m);
            }, catalogueCode, groupCode, subGroupCode, languageCode);
            return rc;
        }
        public  List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, string languageCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p5 AND TD.COD = TABLE_DSC_COD
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
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode, subSubGroupCode, languageCode);

            return drawings;
        }
        public  List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, string languageCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p4 AND TD.COD = TABLE_DSC_COD
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
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode, languageCode);

            return drawings;
        }
        public  List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p3 AND TD.COD = TABLE_DSC_COD
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
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8)

                };
                drawings.Add(language);
            }, catalogueCode, groupCode, languageCode);

            return drawings;
        }
        public  TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
        {
            var t = new TableModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            t.CatalogueCode = catalogueCode;
            t.GroupCode = groupCode;
            t.SubGroupCode = subGroupCode;
            t.SubSubGroupCode = sgsCode;
            //            t.MakeDesc = GetMakeDescription(makeCode, connection);
            //          t.ModelDesc = GetModelDescription(makeCode, modelCode, connection);
            //        t.CatalogueDesc = GetCatalogueDescription(makeCode, modelCode, catalogueCode, connection);
            t.GroupDesc = GetGroupDescription(groupCode, languageCode, connection);
            t.SubGroupDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
            // TODO Add variant information to sgs description
            t.SgsDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
            t.Parts = GetTableParts(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision, languageCode, connection);
            t.DrawingNumbers = GetDrawingNumbers(catalogueCode, groupCode, subGroupCode, sgsCode, revision, connection);
            // t.Narratives = GetSgsNarrative(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
            t.CurrentDrawing = drawingNumber;
            return t;
        }
        private static List<TablePartModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode, SqliteConnection connection)
        {
            var parts = new List<TablePartModel>();
            var sql = @"SELECT TBD_RIF, PRT_COD, TBD_QTY, CDS_DSC, TBD_NOTE1, TBD_NOTE2, TBD_NOTE3,
                                        TBD_SEQ, NTS_DSC, TBD_VAL_FORMULA, DAD.DSC, MODIF
                                        FROM TBDATA
                                        JOIN CODES_DSC ON TBDATA.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $p1
                                        LEFT OUTER JOIN NOTES_DSC ON NOTES_DSC.NTS_COD = TBDATA.NTS_COD AND NOTES_DSC.LNG_COD = $p1
                                        LEFT OUTER JOIN DESC_AGG_DSC DAD ON DAD.COD = TBD_AGG_DSC AND DAD.LNG_COD = $p1
                                        WHERE VARIANTE = $p2 AND SGS_COD = $p3 AND SGRP_COD = $p4 AND GRP_COD = $p5 AND CAT_COD = $p6 AND REVISIONE = $p7
                                        ORDER BY TBD_RIF,TBD_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var part = new TablePartModel
                {
                    PartNumber = reader.GetString(1),
                    TableOrder = reader.GetInt32(0),
                    Quantity = reader.GetString(2),
                    Description = reader.GetString(3),
                    Notes1 = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Notes2 = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Notes3 = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Sequence = reader.GetString(7),
                    Notes = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    Compatibility = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    FurtherDescription = reader.IsDBNull(10) ? "" : reader.GetString(10).ToString(),
                };
                if (!reader.IsDBNull(11))
                    part.Modifications = GetPartModifications(catalogueCode, reader.GetString(11), languageCode, connection);
                parts.Add(part);

            }, languageCode, drawingNumber, sgsCode, subGroupCode, groupCode, catalogueCode, revision);

            foreach (var part in parts)
            {
                part.IsAComponent = IsPartAComponent(part, connection);
            }
            return parts;
        }
        private static List<ModificationModel> GetPartModifications(string catalogueCode, string modifString, string languageCode, SqliteConnection connection)
        {
            var modifications = new List<ModificationModel>();
            if (string.IsNullOrEmpty(modifString)) return modifications;
            var mods = modifString.Split(',');
            var sequence = 1;
            foreach (var mod in mods)
            {
                var type = mod[..1];
                var code = mod[1..];
                var newMod = new ModificationModel
                {
                    Code = int.Parse(code),
                    Type = type,
                    Progression = sequence++
                };
                var sql = "SELECT MDF_DSC FROM MODIF_DSC WHERE CAT_COD = $p1 AND MDF_COD = $p2 AND LNG_COD = $p3";
                connection.RunSqlFirstRowOnly(sql, (reader) => { newMod.Description = reader.GetString(0); }, catalogueCode, newMod.Code, languageCode);
                modifications.Add(newMod);
            }
            foreach (var mod in modifications)
            {
                mod.Activations = GetActivationsForModification(catalogueCode, mod.Code, languageCode, connection);
            }
            return modifications;
        }
        private static List<ActivationModel> GetActivationsForModification(string catalogueCode, int modCode, string languageCode, SqliteConnection connection)
        {
            var modifications = new List<ActivationModel>();
            var sql = @"SELECT IFNULL(A.ACT_MASK, ''),IFNULL(M.MDFACT_SPEC, ''), IFNULL(M.ACT_COD, ''), '', '', '',
                    IFNULL(V.VMK_TYPE, ''), IFNULL(V.VMK_COD, ''), IFNULL(V.VMK_DSC, '')
                    FROM MDF_ACT M
                    LEFT OUTER JOIN ACTIVATIONS A ON A.ACT_COD = M.ACT_COD
                    LEFT OUTER JOIN VMK_DSC V ON V.CAT_COD = M.CAT_COD AND V.VMK_TYPE = M.VMK_TYPE AND V.VMK_COD = M.VMK_COD AND V.LNG_COD = $p2
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

        public List<ModelModel> GetAllVinModels()
        {
            var rc = new List<ModelModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            //            var sql = @"SELECT MOD_COD, MOD_DSC, MK_COD FROM MODELS ORDER BY MOD_SORT_KEY ";
            var sql = @"SELECT CMG_COD, CMG_DSC, MK2_COD FROM COMM_MODGRP ORDER BY CMG_DSC ";
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
        private static List<int> GetDrawingNumbers(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int revision, SqliteConnection connection)
        {
            var rc = new List<int>();
            var sql = @"SELECT DISTINCT VARIANTE 
                            FROM DRAWINGS  
                            WHERE SGS_COD = $p1 AND SGRP_COD = $p2 AND GRP_COD = $p3 AND CAT_COD = $p4 AND REVISIONE = $p5";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                rc.Add(reader.GetInt32(0));
            }, sgsCode, subGroupCode, groupCode, catalogueCode, revision);
            return rc;
        }

        private static bool IsPartAComponent(TablePartModel part, SqliteConnection connection)
        {
            var rc = false;
            var sql = @"SELECT DISTINCT CPLX_PRT_COD FROM CPXDATA WHERE CPLX_PRT_COD = $p1";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = true;

            }, part.PartNumber);
            return rc;
        }
        private static string GetCatalogueDescription(string makeCode, string subMakeCode, string catalogueCode, SqliteConnection connection)
        {
            string rc = "";

            var sql = @"SELECT CAT_DSC FROM CATALOGUES WHERE MK_COD = $p1 AND MK2_COD = $p2 AND CAT_COD = $p3";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode, subMakeCode, catalogueCode);

            return rc;
        }

        protected static string GetGroupDescription(int groupCode, string languageCode, SqliteConnection connection)
        {
            string rc = "";
            var sql = @"SELECT GRP_DSC FROM GROUPS_DSC WHERE GRP_COD = $p1 AND LNG_COD = $p2 ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, languageCode);
            return rc;
        }

        private static string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT SGRP_DSC FROM SUBGROUPS_DSC WHERE SGRP_COD = $p2 AND GRP_COD = $p1 AND LNG_COD = $p3 ";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, subGroupCode, languageCode);
            return rc;
        }
        private static string GetSubSubGroupDescription(string catalogCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"select distinct TD.DSC FROM DRAWINGS T
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p5 AND TD.COD = T.TABLE_DSC_COD
                            WHERE CAT_COD = $p1 AND T.GRP_COD = $p2 AND T.SGRP_COD = $p3 AND T.SGS_COD = $p4
                            ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catalogCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            return rc;
        }


        private static string GetModelDescription(string makeCode, string subMakeCode, string modelCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT CMG_DSC FROM COMM_MODGRP WHERE MK2_COD = $p1 AND CMG_COD = $p2";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, subMakeCode, modelCode);
            return rc;
        }
        public List<ModelModel> GetAllModels()
        {
            var rc = new List<ModelModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            //            var sql = @"SELECT MOD_COD, MOD_DSC, MK_COD FROM MODELS ORDER BY MOD_SORT_KEY ";
            var sql = @"SELECT CMG_COD, CMG_DSC, MK2_COD FROM COMM_MODGRP ORDER BY CMG_SORT_KEY ";
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
        public List<ModelModel> GetAllModelsForMake(string makeCode, string subMake)
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
                    MakeCode = makeCode,
                    SubMakeCode = subMake
                };
                rc.Add(m);
            }, subMake);
            return rc;
        }

        public List<CatalogueModel> GetAllCatalogues(string makeCode, string subMakeCode, string modelCode, string languageCode)
        {
            var rc = new List<CatalogueModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT CAT_COD, CAT_DSC FROM CATALOGUES WHERE MK2_COD = $p2 AND MK_COD =$p1 AND CMG_COD = $p3  ORDER BY CAT_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new CatalogueModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = makeCode,
                    SubMakeCode = subMakeCode,
                    ModelCode = modelCode
                };
                rc.Add(m);
            }, makeCode, subMakeCode, modelCode);
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var rc = new List<GroupModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select distinct T.GRP_COD, GRP_DSC FROM DRAWINGS T
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
            var sql = @"select distinct T.SGRP_COD, SGRP_DSC FROM DRAWINGS T
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

        private static List<ModificationModel> AddSgsModifications(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
        {
            var sql = @"select SGSMOD_CD,S.MDF_COD,MDF_DSC from SGS_MOD S
                        JOIN MODIF_DSC M ON M.MDF_COD = S.MDF_COD 
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            var rc = new List<ModificationModel>();
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new ModificationModel
                {
                    Type = reader.GetString(0),
                    Code = reader.GetInt32(1),
                    Description = reader.GetString(2)
                };
                rc.Add(mod);
            }, catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
            return rc;
        }

        private static List<OptionModel> AddSgsOptions(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
        {
            var sql = @"select O.OPTK_TYPE,O.OPTK_COD,OPTK_DSC from SGS_OPT S
                        JOIN OPTKEYS_DSC O ON O.OPTK_TYPE = S.OPTK_TYPE AND O.OPTK_COD = S.OPTK_COD 
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            var rc = new List<OptionModel>();
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new OptionModel()
                {
                    OptionType = reader.GetString(0),
                    OptionCode = reader.GetString(1),
                    OptionDescription = reader.GetString(2)
                };
                rc.Add(mod);
            }, catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
            return rc;
        }

        private static List<VariationModel> AddSgsVariations(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
        {
            var sql = @"select V.VMK_TYPE,V.VMK_COD,VMK_DSC from SGS_VAL S
                        JOIN VMK_DSC V ON S.VMK_TYPE = V.VMK_TYPE AND S.VMK_COD = V.VMK_COD AND S.CAT_COD = V.CAT_COD
                        where S.CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD = $p3 AND SGS_COD = $p4 AND LNG_COD = $p5
                        ";
            var rc = new List<VariationModel>();
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new VariationModel()
                {
                    VariationType = reader.GetString(0),
                    VariationCode = reader.GetString(1),
                    VariationDescription = reader.GetString(2)
                };
                rc.Add(mod);
            }, catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
            return rc;
        }

        public MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            var m = new MvsModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select M.MOD_COD, M.MVS_VERSION, M.MVS_SERIE, MVS_DSC, MVS_SINCOM_VERS,MVS_ENGINE_TYPE, MV.VMK_DSC, VV.VMK_DSC,
                        M.VMK_TYPE_M||VMK_COD_M, M.VMK_TYPE_V||VMK_COD_V,
                        CT.CAT_COD, CT.CAT_DSC, IFNULL(COL.DSC_COLORE_INT_VET, ''), MM.MOD_DSC, CT.MK_COD, CT.MK2_COD, CT.CMG_COD 
                        from MVS M 
                        JOIN MODELS MM ON MM.MOD_COD = M.MOD_COD
                        JOIN CATALOGUES CT ON CT.CAT_COD = M.CAT_COD
						LEFT OUTER JOIN VMK_DSC MV ON MV.CAT_COD = M.CAT_COD AND MV.VMK_TYPE = M.VMK_TYPE_M AND MV.VMK_COD = M.VMK_COD_M AND MV.LNG_COD = $p1
						LEFT OUTER JOIN VMK_DSC VV ON VV.CAT_COD = M.CAT_COD AND VV.VMK_TYPE = M.VMK_TYPE_V AND VV.VMK_COD = M.VMK_COD_V AND VV.LNG_COD = $p1
                        LEFT OUTER JOIN INTERNAL_COLOURS_DSC COL ON COL.CAT_COD = CT.CAT_COD AND COD_COLORE_INT_VET = $p5 AND COL.LNG_COD = $p1
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
                m.MakeCode = reader.GetString(14);
                m.SubMakeCode = reader.GetString(15);
                m.ModelCode = reader.GetString(16);
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
        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode, string languageCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, REVISIONE, DSC
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p2 AND TD.COD = TABLE_DSC_COD
                            WHERE CAT_COD = $p1
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, VARIANTE, REVISIONE";
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
                    Variant = reader.GetInt32(4),
                    Revision = reader.GetInt32(5),
                    Description = reader.GetString(6)
                };
                drawings.Add(language);
            }, catalogueCode, languageCode);
            return drawings;
        }
        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            if (breadcrumb.MakeCode != null) breadcrumb.MakeDescription = GetMakeDescription(breadcrumb.MakeCode, connection);
            if (breadcrumb.SubMakeCode != null) breadcrumb.SubMakeDescription = GetSubMakeDescription(breadcrumb.MakeCode, breadcrumb.SubMakeCode, connection);
            if (breadcrumb.ModelCode != null) breadcrumb.ModelDescription = GetModelDescription(breadcrumb.MakeCode, breadcrumb.SubMakeCode, breadcrumb.ModelCode, connection);

            if (breadcrumb.CatalogueCode != null) breadcrumb.CatalogueDescription = GetCatalogueDescription(breadcrumb.MakeCode, breadcrumb.SubMakeCode,
                breadcrumb.CatalogueCode, connection);
            if (breadcrumb.GroupCode != null) breadcrumb.GroupDescription = GetGroupDescription(breadcrumb.GroupCode.Value, languageCode, connection);
            if (breadcrumb.GroupCode != null && breadcrumb.SubGroupCode != null) breadcrumb.SubGroupDescription = GetSubGroupDescription(breadcrumb.GroupCode.Value, breadcrumb.SubGroupCode.Value, languageCode, connection);
            if (breadcrumb.GroupCode != null && breadcrumb.SubGroupCode != null && breadcrumb.SubSubGroupCode != null) breadcrumb.SubSubGroupDescription = GetSubSubGroupDescription(breadcrumb.CatalogueCode, breadcrumb.GroupCode.Value, breadcrumb.SubGroupCode.Value, breadcrumb.SubSubGroupCode.Value, languageCode, connection);
        }
        public List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, string clichePartNumber)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CPD_NUM, CLH_COD
                            FROM CLICHE
                            WHERE CPLX_PRT_COD = $p1
                            ORDER BY CPD_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = catalogueCode,
                    GroupCode = groupCode,
                    SubGroupCode = subGroupCode,
                    SubSubGroupCode = subSubGroupCode,
                    ClichePartNumber = clichePartNumber,
                    ClichePartDrawingNumber = reader.GetInt32(0),
                    ClichePartCode = reader.GetString(1)
                };
                drawings.Add(language);
            }, clichePartNumber);
            return drawings;
        }

        public string GetImageNameForClicheDrawing(string clichePartNumber, int clichePartDrawingNumber)
        {
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select DISTINCT IMG_PATH FROM CLICHE
                        where CPLX_PRT_COD = $p1 and CPD_NUM = $p2
                        ";
            var rc = "";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, clichePartNumber, clichePartDrawingNumber);
            return rc;
        }

        public List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber,
            int clicheDrawingNumber, string languageCode)
        {
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var rc = new List<TablePartModel>();
            var sql = @"SELECT C.PRT_COD, CLH_COD, CPD_RIF, CPD_QTY, IFNULL(CPD_AGG_DSC, ''), CDS_DSC
                        FROM CPXDATA C 
                        JOIN PARTS P ON P.PRT_COD = C.PRT_COD
                        JOIN CODES_DSC ON P.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $p3

                        WHERE C.CPLX_PRT_COD = $p1 AND C.CPD_NUM = $p2
                        ORDER BY CPD_RIF, CPD_RIF_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                rc.Add(new TablePartModel
                {
                    PartNumber = reader.GetString(0),
                    TableOrder = reader.GetInt32(2),
                    Quantity = reader.GetString(3),
                    FurtherDescription = reader.GetString(4),
                    Description = reader.GetString(5)
                });
            }, clichePartNumber, clicheDrawingNumber, languageCode);

            var maxRIF = 1;
            if (rc.Count > 0)
                maxRIF = rc.Max(x => x.TableOrder) + 1;
            // Now get any kits
            sql = @"SELECT K.PRT_COD, CDS_DSC
                        FROM KIT K
                        JOIN PARTS P ON P.PRT_COD = K.PRT_COD
                        JOIN CODES_DSC ON P.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $p3
                        WHERE K.CPLX_PRT_COD = $p1 AND K.CAT_COD = $p2
                        ORDER BY TBD_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                rc.Add(new TablePartModel
                {
                    PartNumber = reader.GetString(0),
                    TableOrder = maxRIF++,
                    Quantity = "01",
                    FurtherDescription = "",
                    Description = reader.GetString(1)
                });
            }, clichePartNumber, catalogueCode, languageCode);


            return rc;
        }
        private string GetSubMakeDescription(string makeCode, string subMakeCode, SqliteConnection connection)
        {
            var allMakes = GetAllMakes();
            return allMakes.FirstOrDefault(x => x.SubCode == subMakeCode)?.Description;
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
                        PartNumber = reader.GetString(0),
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

        private List<PartDrawing> GetDrawingsForPartNumber(string partNumber, string languageCode)
        {
            var drawings = new List<PartDrawing>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select A.CAT_COD, C.CAT_DSC, A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, MK_COD, CMG_COD, MK2_COD, REVISIONE  
                            FROM APPLICABILITY A
							JOIN CATALOGUES C ON C.CAT_COD = A.CAT_COD
							JOIN TBDATA T ON T.PRT_COD = A.PRT_COD AND T.CAT_COD = A.CAT_COD AND T.GRP_COD = A.GRP_COD AND T.SGRP_COD = A.SGRP_COD
							JOIN SUBGROUPS_DSC SD ON SD.SGRP_COD = T.SGRP_COD AND SD.GRP_COD = T.GRP_COD AND SD.LNG_COD = $p2
							where A.prt_cod = $p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    Revision = reader.GetInt32(10),
                    ClichePart = false
                };
                drawings.Add(p);
            }, partNumber, languageCode);

            // This could be a part in a cliche
            sql = @"select DISTINCT 
	                A.CAT_COD, CT.CAT_DSC , A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, CT.MK_COD, CMG_COD, MK2_COD,
	                CDS.CDS_DSC, T.PRT_COD, C.PRT_COD, C.CPD_NUM, T.REVISIONE
                        from CPXDATA C
                        JOIN APPLICABILITY A ON C.PRT_COD = A.PRT_COD
                        JOIN SUBGROUPS_DSC SD ON SD.GRP_COD = T.GRP_COD AND SD.SGRP_COD = T.SGRP_COD AND SD.LNG_COD = $p2
                        JOIN CATALOGUES CT ON CT.CAT_COD = A.CAT_COD
                        JOIN CODES_DSC CDS ON CDS.CDS_COD = A.CDS_COD AND CDS.LNG_COD = $p2
                        JOIN TBDATA T ON T.PRT_COD = C.CPLX_PRT_COD AND T.CAT_COD = A.CAT_COD
	                        AND A.GRP_COD = T.GRP_COD AND A.SGRP_COD = T.SGRP_COD

                        WHERE C.PRT_COD = $p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    ClichePartNumber = reader.GetDecimal(11),
                    ClichePartDrawingNumber = reader.GetInt32(13),
                    Revision = reader.GetInt32(14),
                    ClichePart = true
                };
                drawings.Add(p);
            }, partNumber, languageCode);

            return drawings;
        }


    }
}
