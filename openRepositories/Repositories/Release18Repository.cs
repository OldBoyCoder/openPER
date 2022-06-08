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

    }
}
