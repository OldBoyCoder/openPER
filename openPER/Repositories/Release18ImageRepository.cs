﻿using Microsoft.Extensions.Configuration;
using openPER.Interfaces;
using openPER.Models;
using System;
using System.Linq;

namespace openPER.Repositories
{
    public class Release18ImageRepository:IImageRepository
    {
        IConfiguration _config;
        private string _pathToDb;
        public Release18ImageRepository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 84);
            if (release != null)
            {
                _pathToDb = System.IO.Path.Combine(release.FolderName, release.DbName);
            }
        }

        public byte[] GetImageForCatalogue(string cmgCode)
        {
            // Generate file name
            var _basePath = @"C:\ePer installs\Release 18";
            var lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(_basePath, @"SP.IM.00900.FXXXX\img.conf"));
            var matches = lines.Where(x => x.Contains($",{cmgCode},"));
            var line = matches.FirstOrDefault(x => x.Contains("s2"));
            if (line == null)
            {
                line = matches.FirstOrDefault(x => x.Contains("s1"));
                if (line == null) return null;
            }
            var fileName = line.Split(new[] { ',' })[3];
            return System.IO.File.ReadAllBytes(System.IO.Path.Combine(_basePath, @"SP.IM.00900.FXXXX", fileName));
        }
        private byte[] GetImageForGroup(string mapName)
        {
            var _basePath = @"C:\ePer installs\Release 18";
            // Generate file name
            var fileName = System.IO.Path.Combine(_basePath, "SP.MP.00900.FCTLR", $"{mapName}.jpg");
            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            return fileBytes;
        }
        private static byte[] GetImageFromNaFile(string fileName, string imageName, bool wantThumbail)
        {
            var reader = new System.IO.BinaryReader(System.IO.File.OpenRead(fileName));
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
                        reader.BaseStream.Seek(thumbImageStart, System.IO.SeekOrigin.Begin);
                        return reader.ReadBytes(thumbImageLength);

                    }
                    else
                    {
                        reader.BaseStream.Seek(mainImageStart, System.IO.SeekOrigin.Begin);
                        return reader.ReadBytes(mainImageLength);

                    }
                }
            }
            return null;
        }


    }
}