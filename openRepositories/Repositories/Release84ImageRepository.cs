using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
{
    internal class Release84ImageRepository :IImageRepository
    {
        IConfiguration _config;
        private readonly string _pathToImages;
        public Release84ImageRepository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 84);
            if (release != null)
            {
                _pathToImages = release.FolderName;
            }
        }
        public byte[] GetImageForModel(string makeCode, string subMakeCode, string cmgCode)
        {
            return System.IO.File.ReadAllBytes(System.IO.Path.Combine(_pathToImages, $"ModelImages{subMakeCode}", $"{cmgCode}"));
        }

        public byte[] GetImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode,
            string mapName, string imageName)
        {
            // These are in a zip file
            var fileParts = imageName.Split('/');
            var zipFileName = System.IO.Path.Combine(_pathToImages, "ResFiles", $"{fileParts[0]}.res");
            using var zip = ZipFile.Open(zipFileName, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                var e = zip.GetEntry(fileParts[1]);
                if (e == null)
                {
                    return GetImageFromEperFig(imageName);
                }

                using var stream = e.Open();
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }

            return null;

        }
        private byte[] GetImageFromEperFig(string imagePath)
        {
            var parts = imagePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            // Work out filename for zip file
            
            var fileName = System.IO.Path.Combine(_pathToImages, "ResFiles", $"L_EPERFIG.res");
            using (var file = File.OpenRead(fileName))
            {
                using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    var e = zip.GetEntry(imagePath);
                    if (e == null)
                    {
                        Console.WriteLine(imagePath);
                        return null;
                    }
                    using (var stream = e.Open())
                    {
                        var ms = new MemoryStream();
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }
                }
            }
        }



        public byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode,
            int subSubGroupCode, int drawingNumber)
        {
            throw new NotImplementedException();
        }

        public byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode,
            int subSubGroupCode, int drawingNumber)
        {
            throw new NotImplementedException();
        }
    }
}
