using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

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
                model.MapName = (reader.IsDBNull(0))?"":reader.GetString(0);
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

        public override  MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
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

        public override List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode)
        {
            var map = new List<SubGroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            // sloppy should pass this down
            var mapDetails = GetMapForCatalogueGroup(null, null, null, catalogueCode, groupCode);
            var sql = @"select POINT_X, POINT_Y, SGRP_COD from MAP_SGRP M
                        WHERE M.GRP_COD = $p1 AND MAP_NAME = $p2 AND SGRP_COD IN (
                            select distinct T.SGRP_COD FROM TBDATA T
                            WHERE CAT_COD = $p3 AND T.GRP_COD = $p1)
";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubGroupImageMapEntryModel
                {
                    X = reader.GetInt32(0),
                    Y = reader.GetInt32(1),
                    GroupCode = groupCode,
                    SubGroupCode = reader.GetInt32(2)
                };
                map.Add(m);
            }, groupCode, mapDetails.MapName, catalogueCode);
            return map;
        }

        public override string GetImageNameForDrawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup,
            int drawing)
        {
            string rc = "";
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT IMG_PATH
                            FROM DRAWINGS
                            WHERE CAT_COD = $p1 AND GRP_COD = $p2 AND SGRP_COD  = $p3 AND SGS_COD = $p4 AND DRW_NUM = $p5";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catalogue, group, subgroup, subSubGroup, drawing);
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
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, DRW_NUM 
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
                    DrawingNumber = reader.GetInt32(4)
                };
                drawings.Add(language);
            }, catalogueCode, groupCode, subGroupCode, subSubGroupCode);

            return drawings;
        }

    }
}
