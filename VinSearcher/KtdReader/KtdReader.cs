using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace VinSearcher.KtdReader
{
    public partial class KtdReader
    {
        private DbTableHeader _header;
        public IndexBlock FindIndexBlockForKey(string pathToDataFile, string searchKey)
        {
            var indexBlocks = GetIndexBlocks(pathToDataFile, 1);
            foreach (var indexBlock in indexBlocks)
            {
                if (string.Compare(indexBlock.Key, searchKey, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return indexBlock;
                }
            }
            return null;

        }
        public List<string> RecordsForKey(string pathToDataFile, string searchKey, int lineLengthInBytes, IReturnedDataHandler dataHandler)
        {
            var indexBlock = FindIndexBlockForKey(pathToDataFile, searchKey);
            if (indexBlock == null) return null;
            using var fileAccessor = new BinaryReader(new FileStream(pathToDataFile, FileMode.Open, FileAccess.Read, FileShare.Read));
            var uc = UnpackBlock(fileAccessor, indexBlock.Block);
            var record = GetAllRecordsInBlock(searchKey,uc, lineLengthInBytes, dataHandler.ProcessRow);
            return record;
        }
        public List<IndexBlock> GetIndexBlocks(string pathToDataFile, int lineLengthInBytes)
        {
            List<IndexBlock> indexBlocks;
            try
            {
                using var fileAccessor = new BinaryReader(new FileStream(pathToDataFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                _header = new DbTableHeader(fileAccessor);
                if (_header.GetDatabaseVersion() != "F3")
                {
                    throw new Exception("Inadequate version of KTD database.");
                }
                if (_header.ColumnItems.Count > 30)
                {
                    throw new Exception("Corrupted or improper database file.");
                }
                var fields = _header.ColumnItems.ToDictionary(field => field.FieldName, field => "TEXT");

                indexBlocks = GetAllPrimaryIndexBlocks(fileAccessor);
                fileAccessor.Close();
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Database file is not found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while reading the database table. {ex.Message}");
            }
            return indexBlocks;
        }
        public void GetAllData(string pathToDataFile, int lineLengthInBytes)
        {
            try
            {
                using var fileAccessor = new BinaryReader(new FileStream(pathToDataFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                _header = new DbTableHeader(fileAccessor);
                if (_header.GetDatabaseVersion() != "F3")
                {
                    throw new Exception("Inadequate version of KTD database.");
                }
                if (_header.ColumnItems.Count > 30)
                {
                    throw new Exception("Corrupted or improper database file.");
                }
                var fields = _header.ColumnItems.ToDictionary(field => field.FieldName, field => "TEXT");

                var blocks = GetAllPrimaryIndexBlocks(fileAccessor);
                for (var i = 0; i < blocks.Count; i++)
                {
                    if (i % 500 == 0) Console.WriteLine($"{i}/{blocks.Count}");
                    var uc = UnpackBlock(fileAccessor, blocks[i].Block);
                    GetAllRecordsInBlock(uc, lineLengthInBytes, null);
                }
                fileAccessor.Close();
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Database file is not found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while reading the database table. {ex.Message}");
            }
        }

        private List<string> GetAllRecordsInBlock(string searchKey, MemoryStream unpackedContent, int length, Func<List<string>, string, bool> processRow)
        {
            do
            {
                var dataLength = unpackedContent.ReadByte();
                if (length == 2)
                    dataLength += unpackedContent.ReadByte() << 8;
                var content = new byte[dataLength - length];
                var rc = unpackedContent.Read(content, 0, dataLength - length);
                if (rc != dataLength - length)
                    throw new IOException($"Insufficient bytes read when processing searching for key{searchKey}");
                var record = UnpackToList(content, dataLength - length);
                if (processRow(record, searchKey))
                {
                    return record;
                }
            } while (unpackedContent.Position < unpackedContent.Length);
            return null;
        }

        public IEnumerable<string> GetFieldNames(string filename, int lineLengthInBytes)
        {
            try
            {
                using var fileAccessor = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
                _header = new DbTableHeader(fileAccessor);
                if (_header.GetDatabaseVersion() != "F3")
                {
                    throw new Exception("Inadequate version of KTD database.");
                }
                if (_header.ColumnItems.Count > 30)
                {
                    throw new Exception("Corrupted or improper database file.");
                }
                return _header.ColumnItems.Select(x => x.FieldName);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("Database file is not found.");
            }
            catch (Exception)
            {
                throw new Exception("Error while reading the database table.");
            }
        }

        private List<IndexBlock> GetAllPrimaryIndexBlocks(BinaryReader fin)
        {
            var rc = new List<IndexBlock>();
            fin.BaseStream.Seek(_header.PrimaryKeyTablePosition, SeekOrigin.Begin);
            long numberOfLocations = fin.ReadUInt32();
            for (var i = 0; i < numberOfLocations; i++)
            {
                var key = fin.ReadBytes(_header.PrimaryKeyLength);
                long blockStart = fin.ReadUInt32();
                long blockEnd = fin.ReadUInt32();
                var indexBlock = new IndexBlock
                {
                    Block = new BlockSize(blockStart, blockEnd),
                    Key = Encoding.ASCII.GetString(key)
                };

                rc.Add(indexBlock);
            }
            return rc;
        }


        private static MemoryStream UnpackBlock(BinaryReader fin, BlockSize bSize)
        {
            fin.BaseStream.Seek(bSize.Start, SeekOrigin.Begin);
            var result = fin.ReadBytes((int)(bSize.End - bSize.Start));
            var inMs = new MemoryStream(result);
            var outMs = new MemoryStream();
            BZip2.Decompress(inMs, outMs, false);
            outMs.Position = 0;
            return outMs;
        }

        private void GetAllRecordsInBlock(Stream unpackedContent, int length, TextWriter tw)
        {
            do
            {
                var dataLength = unpackedContent.ReadByte();
                if (length == 2)
                    dataLength += unpackedContent.ReadByte() << 8;
                var content = new byte[dataLength - length];
                var rc = unpackedContent.Read(content, 0, dataLength - length);
                if (rc != dataLength - length)
                    throw new IOException("Insufficient bytes read when processing K3 file");
                var record = Unpack(content, dataLength - length);
                foreach (var value in record.Values)
                {
                    tw.Write($"{value}\t");
                }
                tw.WriteLine();
            } while (unpackedContent.Position < unpackedContent.Length);

        }

        private Dictionary<string, string> Unpack(byte[] line, int length)
        {
            var record = new Dictionary<string, string>();
            var unpackedData = UnpackRecord(line, length);
            foreach (var columnItem in _header.ColumnItems)
            {
                var colLength = columnItem.Length;
                if (colLength == 0)
                    colLength = unpackedData.Length - columnItem.StartPosition;
                record.Add(columnItem.FieldName, Encoding.ASCII.GetString(unpackedData, columnItem.StartPosition, colLength));
            }
            return record;
        }
        private List<string> UnpackToList(byte[] line, int length)
        {
            var record = new List<string>();
            var unpackedData = UnpackRecord(line, length);
            foreach (var columnItem in _header.ColumnItems)
            {
                var colLength = columnItem.Length;
                if (colLength == 0)
                    colLength = unpackedData.Length - columnItem.StartPosition;
                record.Add(Encoding.ASCII.GetString(unpackedData, columnItem.StartPosition, colLength));
            }
            return record;
        }
        private byte[] UnpackRecord(byte[] line, int length)
        {
            var result = new byte[_header.GetUnpackedRecordSize(length)];
            foreach (var columnItem in _header.ColumnItems)
            {
                switch (columnItem.DataType)
                {
                    case DbDataType.DTypeReference:
                        {
                            var refIndex = BitConverter.ToUInt16(line, columnItem.PackedPosition);
                            var refValue = _header.ReferenceTables[columnItem.ReferenceIndex][refIndex];
                            Array.Copy(refValue, 0, result, columnItem.StartPosition, columnItem.Length);
                            break;
                        }
                    case DbDataType.DTypeString:
                        {
                            var colLength = columnItem.Length;
                            if (colLength == 0)
                            {
                                colLength = result.Length - columnItem.StartPosition;
                            }

                            Array.Copy(line, columnItem.PackedPosition, result, columnItem.StartPosition, colLength);
                            break;
                        }
                    default:
                        throw new Exception($"Unknown data type {columnItem.DataType}");
                }
            }
            return result;
        }
    }
}
