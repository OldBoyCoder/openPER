using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
{
    public class Release84ImageRepository :IImageRepository
    {
        private IConfiguration _config;
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
            return File.ReadAllBytes(Path.Combine(_pathToImages, $"ModelImages{subMakeCode}", $"{cmgCode}"));
        }
        public byte[] GetSmallImageForModel(string makeCode, string subMakeCode, string cmgCode)
        {
            return File.ReadAllBytes(Path.Combine(_pathToImages, $"SmallModelImages{subMakeCode}", $"{cmgCode}"));
        }

        public byte[] GetImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode,
            MapImageModel mapDetails)
        {
            // These are in a zip file
            var fileParts = mapDetails.ImageName.Split('/');
            var zipFileName = Path.Combine(_pathToImages, "ResFiles", $"{fileParts[0]}.res");
            using var zip = ZipFile.Open(zipFileName, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                var e = zip.GetEntry(fileParts[1]);
                if (e == null)
                {
                    return GetImageFromEperFig(mapDetails.ImageName);
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
            // Work out filename for zip file
            
            var fileName = Path.Combine(_pathToImages, "ResFiles", "L_EPERFIG.res");
            using var file = File.OpenRead(fileName);
            using var zip = new ZipArchive(file, ZipArchiveMode.Read);
            var e = zip.GetEntry(imagePath);
            if (e == null)
            {
                return null;
            }

            using var stream = e.Open();
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms.ToArray();
        }



        public byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode,
            int subSubGroupCode, int drawingNumber, string imageName)
        {
            var fileParts = imageName.Split('/');
            var zipFileName = Path.Combine(_pathToImages, "ResFiles", $"{fileParts[0]}.res");
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

        public byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode,
            int subSubGroupCode, int drawingNumber, string imageName)
        {
            var parts = imageName.Split('.');
            imageName = $"{parts[0]}.th.{parts[1]}";
            return GetImageForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode,
                drawingNumber, imageName);
        }

        public byte[] GetThumbnailForCliche(string clichePartNumber, int clichePartDrawingNumber, string imageName)
        {
            return GetThumbnailForDrawing(null, null, null, 0, 0, 0, clichePartDrawingNumber, imageName);
        }

        public byte[] GetImageForCliche(string clichePartNumber, int clichePartDrawingNumber, string imageName)
        {
            return GetImageForDrawing(null, null, null, 0,0,0,clichePartDrawingNumber, imageName);
        }
    }
}
