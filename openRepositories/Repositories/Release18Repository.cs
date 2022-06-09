using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

// ReSharper disable StringLiteralTypo

namespace openPERRepositories.Repositories
{
    public class Release18Repository : BaseRepository
    {
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

        public override MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode)
        {
            var rc = new MapImageModel();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT MAP_NAME
                            FROM CATALOGUES
                            WHERE MK_COD = $p1 AND MK2_COD = $p2 AND CAT_COD = $p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                rc.MapName = reader.GetString(0);
            }, makeCode, subMakeCode, catalogueCode);
            return rc;
        }
        public override List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
        {
            var map = new List<GroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select MPG_TX-MPI_OFFX, MPG_TY-MPI_OFFY, MPG_INDEX, M.GRP_COD, GRP_DSC from CATALOGUES C
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

            return map;
        }

        public override  List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode)
        {
            var map = new List<SubGroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            // sloppy should pass this down
            var mapDetails = GetMapForCatalogueGroup(null, null, null, catalogueCode, groupCode);
            var sql = @"select POINT_X-LABEL_X, POINT_Y-LABEL_Y, SGRP_COD from MAP_SGRP M
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
    }
}
