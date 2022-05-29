using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VinSearcher.KtdReader
{
    internal class DbTableHeader
    {
        private readonly string _dbVersion;
        internal readonly long PrimaryKeyTablePosition;
        internal readonly List<DbTableItem> ColumnItems = new List<DbTableItem>();
        internal readonly int PrimaryKeyLength;
        private int _referenceAddition;
        internal readonly List<List<byte[]>> ReferenceTables = new List<List<byte[]>>();
        internal List<List<string>> Indices = new List<List<string>>();

        public string GetDatabaseVersion()
        {
            return _dbVersion;
        }

        public DbTableHeader(BinaryReader fin)
        {
            int i;
            fin.BaseStream.Seek(0, SeekOrigin.Begin);
            var fType = fin.ReadBytes(2);
            _dbVersion = Encoding.ASCII.GetString(fType);
            fin.ReadUInt32();
            fin.ReadUInt16();
            var referenceTablePositions = new long[5];
            for (i = 0; i < 5; i++)
            {
                referenceTablePositions[i] = fin.ReadUInt32();
            }
            PrimaryKeyTablePosition = fin.ReadUInt32();
            var secondaryKeyTablePositions = new long[3];
            for (i = 0; i < 3; i++)
            {
                secondaryKeyTablePositions[i] = fin.ReadUInt32();
            }

            fin.ReadBytes(20);

            var primaryKeyItems = new DbTableItem[fin.ReadByte()];
            for (i = 0; i < primaryKeyItems.Length; i++)
            {
                primaryKeyItems[i] = new DbTableItem(fin);
            }
            Indices.Add(primaryKeyItems.Select(x => x.FieldName).ToList());
            var secondaryKeyItems = new DbTableItem[fin.ReadByte()][];
            for (var j = 0; j < secondaryKeyItems.Length; j++)
            {
                secondaryKeyItems[j] = new DbTableItem[fin.ReadByte()];
                for (i = 0; i < secondaryKeyItems[j].Length; i++)
                {
                    secondaryKeyItems[j][i] = new DbTableItem(fin);
                }
                Indices.Add(secondaryKeyItems[j].Select(x => x.FieldName).ToList());
            }
            var colCount = fin.ReadByte();
            for (i = 0; i < colCount; i++)
            {
                ColumnItems.Add(new DbTableItem(fin));
            }

            PrimaryKeyLength = primaryKeyItems.Sum(x => x.Length);

            var secondaryKeyLengths = new int[secondaryKeyItems.Length];
            for (var j = 0; j < secondaryKeyLengths.Length; j++)
            {
                secondaryKeyLengths[j] = secondaryKeyItems[j].Sum(x => x.Length);
            }
            var numberOfReferenceTables = 0;
            while (numberOfReferenceTables < referenceTablePositions.Length)
            {
                if (referenceTablePositions[numberOfReferenceTables] == 0L) break;
                ++numberOfReferenceTables;
            }
            ConfigureAdditionalProperties();
            for (i = 0; i < numberOfReferenceTables; i++)
            {
                ReferenceTables.Add(LoadReferenceTable(referenceTablePositions[i], fin));
            }

        }

        private static List<byte[]> LoadReferenceTable(long refTablePosition, BinaryReader fin)
        {
            var referenceTable = new List<byte[]>();
            fin.BaseStream.Seek(refTablePosition, SeekOrigin.Begin);
            var numberOfItems = (int)fin.ReadUInt32();
            var itemFixedWidth = (int)fin.ReadUInt32();
            for (var i = 0; i < numberOfItems; i++)
            {
                var b = fin.ReadBytes(itemFixedWidth);
                referenceTable.Add(b);
            }
            return referenceTable;
        }

        public int GetUnpackedRecordSize(int packedSize)
        {
            return packedSize + _referenceAddition;
        }

        private void ConfigureAdditionalProperties()
        {
            var curPos = 0;
            var curRef = 0;
            foreach (var columnItem in ColumnItems)
            {
                columnItem.PackedPosition = curPos;
                switch (columnItem.DataType)
                {
                    case DbDataType.DTypeReference:
                        columnItem.ReferenceIndex = curRef++;
                        curPos += 2;
                        break;
                    case DbDataType.DTypeString:
                        columnItem.ReferenceIndex = -1;
                        curPos += columnItem.Length;
                        break;
                    default:
                        throw new Exception($"Unknown data type {columnItem.DataType}");
                }
            }
            _referenceAddition = ColumnItems.Where(x => x.DataType == DbDataType.DTypeReference)
                .Sum(x => x.Length - 2);
        }
    }
}

