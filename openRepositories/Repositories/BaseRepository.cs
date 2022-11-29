using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        internal IConfiguration _config;
        internal string _pathToDb;


        public abstract TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber,
            int revision, string languageCode);

        public abstract List<MakeModel> GetAllMakes();
        public abstract string GetMakeDescription(string makeCode, SqliteConnection connection);


        protected List<int> GetDrawingNumbers(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int revision, SqliteConnection connection)
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

        internal bool IsPartAComponent(TablePartModel part, SqliteConnection connection)
        {
            var rc = false;
            var sql = @"SELECT DISTINCT CPLX_PRT_COD FROM CPXDATA WHERE CPLX_PRT_COD = $p1";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = true;

            }, (decimal)part.PartNumber);
            return rc;
        }

        private string GetCatalogueDescription(string makeCode, string subMakeCode, string catalogueCode, SqliteConnection connection)
        {
            string rc = "";

            var sql = @"SELECT CAT_DSC FROM CATALOGUES WHERE MK_COD = $p1 AND MK2_COD = $p2 AND CAT_COD = $p3";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode, subMakeCode, catalogueCode);

            return rc;
        }

        protected string GetGroupDescription(int groupCode, string languageCode, SqliteConnection connection)
        {
            string rc = "";
            var sql = @"SELECT GRP_DSC FROM GROUPS_DSC WHERE GRP_COD = $p1 AND LNG_COD = $p2 ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, languageCode);
            return rc;
        }

        protected string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT SGRP_DSC FROM SUBGROUPS_DSC WHERE SGRP_COD = $p2 AND GRP_COD = $p1 AND LNG_COD = $p3 ";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, subGroupCode, languageCode);
            return rc;
        }
        protected string GetSubSubGroupDescription(string catalogCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"select distinct TD.DSC FROM DRAWINGS T
                            JOIN TABLES_DSC TD ON TD.LNG_COD = $p5 AND TD.COD = T.TABLE_DSC_COD
                            WHERE CAT_COD = $p1 AND T.GRP_COD = $p2 AND T.SGRP_COD = $p3 AND T.SGS_COD = $p4
                            ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catalogCode, groupCode, subGroupCode,subSubGroupCode, languageCode);
            return rc;
        }


        private string GetModelDescription(string makeCode, string subMakeCode, string modelCode, SqliteConnection connection)
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

        public abstract List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode,
            int groupCode, int subGroupCode,
            string languageCode);

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

        protected List<ModificationModel> AddSgsModifications(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
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

        protected List<OptionModel> AddSgsOptions(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
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

        protected List<VariationModel> AddSgsVariations(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, SqliteConnection connection)
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

        public abstract List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode,
            string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode);

        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, REVISIONE
                            FROM DRAWINGS
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
                    Revision = reader.GetInt32(5)
                };
                drawings.Add(language);
            }, catalogueCode);
            return drawings;
        }
        public abstract List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode);

        public abstract List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode);

        public abstract MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode);

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
            if (breadcrumb.GroupCode != null && breadcrumb.SubGroupCode != null && breadcrumb.SubSubGroupCode !=null) breadcrumb.SubSubGroupDescription = GetSubSubGroupDescription(breadcrumb.CatalogueCode, breadcrumb.GroupCode.Value, breadcrumb.SubGroupCode.Value, breadcrumb.SubSubGroupCode.Value, languageCode, connection);
        }

        public abstract List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode);

        public abstract MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model,
            string catalogue, int group);

        public abstract List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode,
            int groupCode, string languageCode);

        public abstract string GetImageNameForDrawing(string make, string model, string catalogue, int group,
            int subgroup, int subSubGroup,
            int drawing, int revision);

        public List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, decimal clichePartNumber)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CPD_NUM, CLH_COD
                            FROM CPXDATA
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
                    ClichePartCode = reader.GetInt32(1)
                };
                drawings.Add(language);
            }, (decimal)clichePartNumber);
            return drawings;
        }

        public string GetImageNameForClicheDrawing(decimal clichePartNumber, int clichePartDrawingNumber)
        {
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select DISTINCT CLH_COD FROM CPXDATA
                        where CPLX_PRT_COD = $p1 and CPD_NUM = $p2
                        ";
            var rc = "";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetInt32(0).ToString();
            }, (decimal)clichePartNumber, clichePartDrawingNumber);
            return rc;
        }

        public List<TablePartModel> GetPartsForCliche(string catalogueCode, decimal clichePartNumber,
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
                    PartNumber = (decimal)reader.GetDecimal(0),
                    TableOrder = reader.GetInt32(2),
                    Quantity = reader.GetString(3),
                    FurtherDescription = reader.GetString(4),
                    Description = reader.GetString(5)
                });
            }, (decimal)clichePartNumber, clicheDrawingNumber, languageCode);

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
                    PartNumber = (decimal)reader.GetDouble(0),
                    TableOrder = maxRIF++,
                    Quantity = "01",
                    FurtherDescription = "",
                    Description = reader.GetString(1)
                });
            }, (decimal)clichePartNumber, catalogueCode, languageCode);


            return rc;
        }

        public abstract List<ModelModel> GetAllVinModels();
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedParameter.Local
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
                        PartNumber = (decimal)reader.GetDecimal(0),
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

        private List<PartDrawing> GetDrawingsForPartNumber(decimal partNumber, string languageCode)
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
            }, (int)partNumber, languageCode);

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
