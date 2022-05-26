using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using openPER.Interfaces;
using openPER.Models;
using System;
using System.Collections.Generic;

namespace openPER.Repositories
{
    public class Release20Repository : IRepository

    {
        private readonly IMemoryCache _cache;
        public Release20Repository(IMemoryCache cache)
        {
            _cache = cache;
        }
        public TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode)
        {
            var t = new TableViewModel();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                t.MakeDesc = GetMakeDescription(makeCode, connection);
                t.ModelDesc = GetModelDescription(makeCode, modelCode, connection);
                t.CatalogueDesc = GetCatalogueDescription(makeCode, modelCode, catalogueCode, connection);
                t.GroupDesc = GetGroupDescription(groupCode, languageCode, connection);
                t.SubGroupDesc = GetSubGroupDescription(groupCode,subGroupCode, languageCode, connection);
                // TODO Add variant information to sgs description
                t.SgsDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
                t.Parts = GetTableParts(catalogueCode, groupCode, subGroupCode, sgsCode, 1,languageCode, connection);
            }
            return t;
        }
        private List<TablePartViewModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode, SqliteConnection connection)
        {
            var parts = new List<TablePartViewModel>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT TBD_RIF, PRT_COD, TBD_QTY, CDS_DSC 
                                        FROM TBDATA
                                        JOIN CODES_DSC ON TBDATA.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $languageCode
                                        WHERE DRW_NUM = $drawingNumber AND SGS_COD = $sgsCode AND SGRP_COD = $subGroupCode AND GRP_COD = $groupCode AND CAT_COD = $catalogueCode 
                                        ORDER BY TBD_SEQ";
            command.Parameters.AddWithValue("$drawingNumber", drawingNumber);
            command.Parameters.AddWithValue("$sgsCode", sgsCode);
            command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
            command.Parameters.AddWithValue("$groupCode", groupCode);
            command.Parameters.AddWithValue("$languageCode", languageCode);
            command.Parameters.AddWithValue("$catalogueCode", catalogueCode);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var part = new TablePartViewModel();
                    part.PartNumber = reader.GetDouble(1);
                    part.TableOrder = reader.GetInt32(0);
                    part.Quantity = reader.GetString(2);
                    part.Description = reader.GetString(3);
                    parts.Add(part);

                }
            }

            return parts;

        }
        private string GetCatalogueDescription(string makeCode, string modelCode, string catalogueCode, SqliteConnection connection)
        {
            var cacheKeys = new { type = "CAT", k1 = modelCode, k2 = catalogueCode };
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT CMD_DSC FROM COMM_MODELS WHERE MOD_COD = $modelCode AND CAT_COD = $catalogueCode ";
                command.Parameters.AddWithValue("$modelCode", modelCode);
                command.Parameters.AddWithValue("$catalogueCode", catalogueCode);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        rc = reader.GetString(0);
                    }
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        private string GetGroupDescription(int groupCode, string languageCode, SqliteConnection connection)
        {
            var cacheKeys = new { type = "GROUP", k1 = languageCode, k2 = groupCode };
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT GRP_DSC FROM GROUPS_DSC WHERE GRP_COD = $groupCode AND LNG_COD = $languageCode ";
                command.Parameters.AddWithValue("$groupCode", groupCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rc = reader.GetString(0);
                    }
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        private string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode, SqliteConnection connection)
        {
            var cacheKeys = new { type = "SUBGROUP", k1 = languageCode, k2 = groupCode, k3=subGroupCode };
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT SGRP_DSC FROM SUBGROUPS_DSC WHERE SGRP_COD = $subGroupCode AND GRP_COD = $groupCode AND LNG_COD = $languageCode ";
                command.Parameters.AddWithValue("$groupCode", groupCode);
                command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rc = reader.GetString(0);
                    }
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }


        private static string GetMakeDescription(string makeCode, SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT MK_DSC FROM MAKES WHERE MK_COD = $makeCode";
            command.Parameters.AddWithValue("$makeCode", makeCode);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "";
        }
        private static string GetModelDescription(string makeCode, string modelCode, SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT MOD_DSC FROM MODELS WHERE MK_COD = $makeCode AND MOD_COD = $modelCode";
            command.Parameters.AddWithValue("$makeCode", makeCode);
            command.Parameters.AddWithValue("$modelCode", modelCode);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "";
        }
    }
}
