using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using openPERModels;

// ReSharper disable StringLiteralTypo

namespace openPERRepositories.Repositories
{
    public class Release84Repository : BaseRepository
    {
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

        public override MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
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
        public override List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
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

        public override MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
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

        public override List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
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

        public override string GetImageNameForDrawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup,
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

        public override List<MakeModel> GetAllMakes()
        {
            var rc = new List<MakeModel>
            {
                new ("F","F", "FIAT"),
                new ("F","T", "FIAT COMMERCIAL"),
                new ("L","L", "LANCIA"),
                new ("R","R", "ALFA ROMEO"),
                new ("F", "C", "ABARTH")
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

        public override string GetMakeDescription(string makeCode, SqliteConnection connection)
        {
            var rc = "";
            var sql = @"SELECT MK_DSC FROM MAKES WHERE MK_COD = $p1";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode);
            return rc;
        }

        public override List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
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
                //m.Modifications = AddSgsModifications(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                // m.Options = AddSgsOptions(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                // m.Variations = AddSgsVariations(catalogueCode, groupCode, subGroupCode, m.Code, languageCode, connection);
                rc.Add(m);
            }, catalogueCode, groupCode, subGroupCode);
            return rc;
        }
        public override List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE
                            FROM DRAWINGS
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
                    Revision = reader.GetInt32(6)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode, subSubGroupCode);

            return drawings;
        }
        public override TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
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
        private List<TablePartModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode, SqliteConnection connection)
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
                    PartNumber = (decimal)reader.GetDouble(1),
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
        private List<ModificationModel> GetPartModifications(string catalogueCode, string modifString, string languageCode, SqliteConnection connection)
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
        private List<ActivationModel> GetActivationsForModification(string catalogueCode, int modCode, string languageCode, SqliteConnection connection)
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

        public override List<ModelModel> GetAllVinModels()
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

    }
}
