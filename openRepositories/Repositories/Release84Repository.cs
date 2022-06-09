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

        public override string GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, out string imageName)
        {
            var map = "";
            var image = "";
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"SELECT DISTINCT MAP_NAME, IMG_NAME
                            FROM CATALOGUES
                            WHERE MK_COD = $p1 AND MK2_COD = $p2 AND CAT_COD = $p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                map = reader.GetString(0);
                image = reader.GetString(1);
            }, makeCode, subMakeCode, catalogueCode);
            imageName = image;
            return map;
        }
        public override List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode)
        {
            var map = new List<GroupImageMapEntryModel>();
            using var connection = new SqliteConnection($"Data Source={_pathToDb}");
            var sql = @"select MPG_TX, MPG_TY, MPG_INDEX, GRP_COD from CATALOGUES C
                        JOIN MAP_GRP M ON M.MAP_NAME = C.MAP_NAME
                        JOIN MAP_INFO MI ON MI.MAP_NAME = C.MAP_NAME
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
                    GroupCode = reader.GetInt32(3)
                };
                map.Add(m);
            }, catalogueCode);
            return map;

        }

    }
}
