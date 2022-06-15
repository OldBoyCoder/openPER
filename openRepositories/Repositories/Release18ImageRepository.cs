using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
{
    public class Release18ImageRepository:IImageRepository
    {
        IConfiguration _config;
        private readonly string _pathToImages;
        public Release18ImageRepository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 18);
            if (release != null)
            {
                _pathToImages = release.FolderName;
            }
        }

        public byte[] GetImageForModel(string makeCode, string subMakeCode, string modelCode)
        {
            // Generate file name
            var lines = File.ReadAllLines(Path.Combine(_pathToImages, $"ModelImages{subMakeCode}", "img.conf"));
            var matches = lines.Where(x => x.Contains($",{modelCode},")).ToList();
            var line = matches.FirstOrDefault(x => x.Contains("s2"));
            if (line == null)
            {
                line = matches.FirstOrDefault(x => x.Contains("s1"));
                if (line == null) return null;
            }
            var folder = Path.Combine(_pathToImages, $"ModelImages{subMakeCode}");
            var file = line.Split(new[] { ',' })[3];
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];

            return File.ReadAllBytes( fileName);
        }
        public byte[] GetSmallImageForModel(string makeCode, string subMakeCode, string modelCode)
        {
            return GetImageForModel(makeCode, subMakeCode, modelCode);
        }

        public byte[] GetImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode,
            MapImageModel mapDetails)
        {
            var folder = Path.Combine(_pathToImages, "CatalogueImages");
            var file = $"{mapDetails.MapName}.jpg";
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];

            return File.ReadAllBytes(fileName);
        }

        public byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode,
            int subSubGroupCode, int drawingNumber, string imageName)
        {
            if (makeCode == "T") makeCode = "F";
            var folder = Path.Combine(_pathToImages, "DrawingImages");
            var file = $"{makeCode}{catalogueCode}.na";
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];
            return GetImageFromNaFile(fileName, imageName, false);
        }

        public byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode,
            int subSubGroupCode, int drawingNumber, string imageName)
        {
            if (makeCode == "T") makeCode = "F";
            var folder = Path.Combine(_pathToImages, "DrawingImages");
            var file = $"{makeCode}{catalogueCode}.na";
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];

            return GetImageFromNaFile(fileName, imageName, true);
        }

        public byte[] GetThumbnailForCliche(double clichePartNumber, int clichePartDrawingNumber, string imageName)
        {
            var folder = Path.Combine(_pathToImages, "DrawingImages");
            var file = "cliche.na";
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];
            imageName = imageName.PadLeft(10, '0');

            return GetImageFromNaFile(fileName, imageName, true);
        }

        public byte[] GetImageForCliche(double clichePartNumber, int clichePartDrawingNumber, string imageName)
        {
            var folder = Path.Combine(_pathToImages, "DrawingImages");
            var file = "cliche.na";
            var files = Directory.GetFiles(folder, file,
                new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            var fileName = files[0];
            imageName = imageName.PadLeft(10, '0');

            return GetImageFromNaFile(fileName, imageName, false);
        }

        private byte[] GetImageForGroup(string mapName)
        {
            var _basePath = @"C:\ePer installs\Release 18";
            // Generate file name
            var fileName = Path.Combine(_basePath, "SP.MP.00900.FCTLR", $"{mapName}.jpg");
            var fileBytes = File.ReadAllBytes(fileName);
            return fileBytes;
        }
        private static byte[] GetImageFromNaFile(string fileName, string imageName, bool wantThumbail)
        {
            var reader = new BinaryReader(File.OpenRead(fileName));
            reader.ReadInt16();
            Int32 numberOfEntries = reader.ReadInt16();
            for (int i = 0; i < numberOfEntries; i++)
            {
                reader.ReadInt16(); // image index
                byte[] imageNameBytes = reader.ReadBytes(10);
                string entry = System.Text.Encoding.ASCII.GetString(imageNameBytes);
                Int32 mainImageStart = reader.ReadInt32();
                Int32 mainImageLength = reader.ReadInt32();
                Int32 thumbImageStart = reader.ReadInt32();
                Int32 thumbImageLength = reader.ReadInt32();
                if (entry == imageName)
                {
                    if (wantThumbail)
                    {
                        reader.BaseStream.Seek(thumbImageStart, SeekOrigin.Begin);
                        return reader.ReadBytes(thumbImageLength);

                    }
                    else
                    {
                        reader.BaseStream.Seek(mainImageStart, SeekOrigin.Begin);
                        return reader.ReadBytes(mainImageLength);

                    }
                }
            }
            return null;
        }


    }
}
