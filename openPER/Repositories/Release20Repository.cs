﻿using Microsoft.Data.Sqlite;
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
        public TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode)
        {
            var t = new TableViewModel();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                t.MakeDesc = GetMakeDescription(makeCode, connection);
                t.ModelDesc = GetModelDescription(makeCode, modelCode, connection);
                t.CatalogueDesc = GetCatalogueDescription(makeCode, modelCode, catalogueCode, connection);
                t.GroupDesc = GetGroupDescription(groupCode, languageCode, connection);
                t.SubGroupDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
                // TODO Add variant information to sgs description
                t.SgsDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode, connection);
                t.Parts = GetTableParts(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode, connection);
                t.DrawingNumbers = GetDrawingNumbers(catalogueCode, groupCode, subGroupCode, sgsCode, connection);
                t.Narratives = GetSgsNarrative(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode);
                t.CurrentDrawing = drawingNumber;

            }
            return t;
        }
        public List<MakeViewModel> GetAllMakes()
        {
            var rc = new List<MakeViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT MK_COD, MK_DSC FROM MAKES ORDER BY MK_DSC ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new MakeViewModel
                        {
                            Code = reader.GetString(0),
                            Description = reader.GetString(1)
                        };
                        rc.Add(m);
                    }
                }

            }
            return rc;

        }

        private List<int> GetDrawingNumbers(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, SqliteConnection connection)
        {
            var rc = new List<int>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT DISTINCT DRW_NUM FROM TBDATA  WHERE SGS_COD = $sgsCode AND SGRP_COD = $subGroupCode AND GRP_COD = $groupCode AND CAT_COD = $catalogueCode ";
            command.Parameters.AddWithValue("$sgsCode", sgsCode);
            command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
            command.Parameters.AddWithValue("$groupCode", groupCode);
            command.Parameters.AddWithValue("$catalogueCode", catalogueCode);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rc.Add(reader.GetInt32(0));
                }
            }
            return rc;

        }

        private List<TablePartViewModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode, SqliteConnection connection)
        {
            var parts = new List<TablePartViewModel>();
            var command = connection.CreateCommand();
            // TODO Modifications and whatever need to be attached lists not a single string.  Redo this sql
            command.CommandText = @"SELECT TBD_RIF, PRT_COD, TBD_QTY, CDS_DSC, TBD_NOTE1, TBD_NOTE2, TBD_NOTE3,
                                        TBD_SEQ, NTS_DSC, TBD_VAL_FORMULA, TBD_AGG_DSC
                                        FROM TBDATA
                                        JOIN CODES_DSC ON TBDATA.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = $languageCode
                                        LEFT OUTER JOIN NOTES_DSC ON NOTES_DSC.NTS_COD = TBDATA.NTS_COD AND NOTES_DSC.LNG_COD = $languageCode
                                        WHERE DRW_NUM = $drawingNumber AND SGS_COD = $sgsCode AND SGRP_COD = $subGroupCode AND GRP_COD = $groupCode AND CAT_COD = $catalogueCode 
                                        ORDER BY TBD_RIF,TBD_SEQ";
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
                    part.Notes1 = reader.IsDBNull(4)? "": reader.GetString(4);
                    part.Notes2 = reader.IsDBNull(5) ? "" : reader.GetString(5);
                    part.Notes3 = reader.IsDBNull(6) ? "" : reader.GetString(6);
                    part.Sequence = reader.GetString(7);
                    part.Notes = reader.IsDBNull(8) ? "" : reader.GetString(8);
                    part.Compatibility = reader.IsDBNull(9) ? "" : reader.GetString(9);
                    part.FurtherDescription = reader.IsDBNull(10) ? "" : reader.GetString(10);
                    parts.Add(part);

                }
            }
            foreach (var part in parts)
            {
                part.Modifications = GetPartModifications(part, catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode, connection);
            }
            return parts;

        }
        private List<ModificationViewModel> GetPartModifications(TablePartViewModel part, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode, SqliteConnection connection)
        {
            var modifications = new List<ModificationViewModel>();
            var command = connection.CreateCommand();
            // TODO Modifications and whatever need to be attached lists not a single string.  Redo this sql
            command.CommandText = @"SELECT TBDM_CD, TBDATA_MOD.MDF_COD, TBDM_PROG , MDF_DSC
                                        FROM TBDATA_MOD
                                        JOIN MODIF_DSC ON TBDATA_MOD.MDF_COD = MODIF_DSC.MDF_COD AND MODIF_DSC.LNG_COD = $languageCode
                                        WHERE DRW_NUM = $drawingNumber AND SGS_COD = $sgsCode AND SGRP_COD = $subGroupCode AND GRP_COD = $groupCode AND CAT_COD = $catalogueCode 
                                            AND TBD_RIF = $rif AND TBD_SEQ = $sequence
                                        ORDER BY TBDM_PROG";
            command.Parameters.AddWithValue("$drawingNumber", drawingNumber);
            command.Parameters.AddWithValue("$sgsCode", sgsCode);
            command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
            command.Parameters.AddWithValue("$groupCode", groupCode);
            command.Parameters.AddWithValue("$languageCode", languageCode);
            command.Parameters.AddWithValue("$catalogueCode", catalogueCode);
            command.Parameters.AddWithValue("$rif", part.TableOrder);
            command.Parameters.AddWithValue("$sequence", part.Sequence);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var mod = new ModificationViewModel();
                    mod.Type = reader.GetString(0);
                    mod.Code = reader.GetInt32(1);
                    mod.Progression = reader.GetInt32(2);
                    mod.Description = reader.GetString(3);
                    modifications.Add(mod);

                }
            }
            // Get activations for modifications
            foreach (var mod in modifications)
            {
                mod.Activations = GetActivationsForModification(catalogueCode, mod.Code, languageCode, connection);
            }
            return modifications;
        }
        private List<ActivationViewModel> GetActivationsForModification(string catalogueCode, int modCode, string languageCode, SqliteConnection connection )
        {
            var modifications = new List<ActivationViewModel>();
            var command = connection.CreateCommand();
            // TODO Modifications and whatever need to be attached lists not a single string.  Redo this sql
            command.CommandText = @"SELECT IFNULL(A.ACT_MASK, ''),IFNULL(M.MDFACT_SPEC, ''), IFNULL(M.ACT_COD, ''), IFNULL(O.OPTK_TYPE, ''), IFNULL(O.OPTK_COD, ''), IFNULL(O.OPTK_DSC, ''),
                    IFNULL(V.VMK_TYPE, ''), IFNULL(V.VMK_COD, ''), IFNULL(V.VMK_DSC, '')
                    FROM MDF_ACT M
                    LEFT OUTER JOIN ACTIVATIONS A ON A.ACT_COD = M.ACT_COD
                    LEFT OUTER JOIN VMK_DSC V ON V.CAT_COD = M.CAT_COD AND V.VMK_TYPE = M.VMK_TYPE AND V.VMK_COD = M.VMK_COD AND V.LNG_COD = $languageCode
                    LEFT OUTER JOIN OPTKEYS_DSC O ON O.OPTK_TYPE = M.OPTK_TYPE AND O.OPTK_COD = M.OPTK_COD AND O.LNG_COD = $languageCode
                    WHERE M.CAT_COD = $catalogueCode AND M.MDF_COD = $modCode";
            command.Parameters.AddWithValue("$modCode", modCode);
            command.Parameters.AddWithValue("$languageCode", languageCode);
            command.Parameters.AddWithValue("$catalogueCode", catalogueCode);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var mod = new ActivationViewModel();
                    mod.ActivationDescription = reader.GetString(0) +" "+reader.GetString(1);
                    mod.ActivationCode = reader.GetString(2);
                    mod.OptionType = reader.GetString(3);
                    mod.OptionCode = reader.GetString(4);
                    mod.OptionDescription = reader.GetString(5);
                    mod.VariationType = reader.GetString(6);
                    mod.VariationCode = reader.GetString(7);
                    mod.VariationDescription = reader.GetString(8);
                    modifications.Add(mod);

                }
            }
            // Get activations for modifications

            return modifications;
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
            var cacheKeys = new { type = "SUBGROUP", k1 = languageCode, k2 = groupCode, k3 = subGroupCode };
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

        public List<ModelViewModel> GetAllModels(string makeCode)
        {
            var rc = new List<ModelViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT CMG_COD, CMG_DSC FROM COMM_MODGRP WHERE MK2_COD = $makeCode  ORDER BY CMG_SORT_KEY ";
                command.Parameters.AddWithValue("$makeCode", makeCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new ModelViewModel
                        {
                            Code = reader.GetString(0),
                            Description = reader.GetString(1),
                            MakeCode = makeCode
                        };
                        rc.Add(m);
                    }
                }

            }
            return rc;
        }

        public List<CatalogueViewModel> GetAllCatalogues(string makeCode, string modelCode, string languageCode)
        {
            var rc = new List<CatalogueViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT CAT_COD, CAT_DSC FROM CATALOGUES WHERE MK2_COD = $makeCode AND CMG_COD = $modelCode  ORDER BY CAT_SORT_KEY ";
                command.Parameters.AddWithValue("$makeCode", makeCode);
                command.Parameters.AddWithValue("$modelCode", modelCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new CatalogueViewModel
                        {
                            Code = reader.GetString(0),
                            Description = reader.GetString(1),
                            MakeCode = makeCode,
                            ModelCode = modelCode
                        };
                        rc.Add(m);
                    }
                }

            }
            foreach (var item in rc)
            {
                item.Groups = GetGroupsForCatalogue(item.Code, languageCode);
            }
            return rc;
        }

        public List<GroupViewModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var rc = new List<GroupViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"select distinct T.GRP_COD, GRP_DSC FROM TBDATA T
                            JOIN GROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.LNG_COD = $languageCode
                            WHERE CAT_COD = $catalogueCode
                            order by T.GRP_COD";
                command.Parameters.AddWithValue("$catalogueCode", catalogueCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new GroupViewModel
                        {
                            Code = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        };
                        rc.Add(m);
                    }
                }

            }
            foreach (var group in rc)
            {
                group.SubGroups = GetSubgroupsForCatalogueGroup(catalogueCode, group.Code, languageCode);
            }
            return rc;
        }

        private List<SubGroupViewModel> GetSubgroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var rc = new List<SubGroupViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"select distinct T.SGRP_COD, SGRP_DSC FROM TBDATA T
                            JOIN SUBGROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.SGRP_COD = T.SGRP_COD AND G.LNG_COD = $languageCode
                            WHERE CAT_COD = $catalogueCode AND T.GRP_COD = $groupCode
                            order by T.SGRP_COD";
                command.Parameters.AddWithValue("$catalogueCode", catalogueCode);
                command.Parameters.AddWithValue("$groupCode", groupCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new SubGroupViewModel
                        {
                            Code = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        };
                        rc.Add(m);
                    }
                }

            }
            foreach (var item in rc)
            {
                item.SgsGroups = GetSgsGroupsForCatalogueGroup(catalogueCode, groupCode, item.Code, languageCode);
            }
            return rc;
        }
        private List<SgsViewModel> GetSgsGroupsForCatalogueGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var rc = new List<SgsViewModel>();
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"select distinct T.SGS_COD FROM TBDATA T
                            WHERE CAT_COD = $catalogueCode AND T.GRP_COD = $groupCode AND T.SGRP_COD = $subGroupCode
                            order by T.SGS_COD";
                command.Parameters.AddWithValue("$catalogueCode", catalogueCode);
                command.Parameters.AddWithValue("$groupCode", groupCode);
                command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var m = new SgsViewModel
                        {
                            Code = reader.GetInt32(0),

                        };
                        m.Narrative = GetSgsNarrative(catalogueCode, groupCode, subGroupCode, m.Code, languageCode);
                        rc.Add(m);
                    }
                }

            }
            return rc;
        }
        private List<string> GetSgsNarrative(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode)
        {
            var rc = new List<string>();
            var sql = @"select V.VMK_TYPE,V.VMK_COD,VMK_DSC from SGS_VAL S
                        JOIN VMK_DSC V ON S.VMK_TYPE = V.VMK_TYPE AND S.VMK_COD = V.VMK_COD AND S.CAT_COD = V.CAT_COD
                        where S.CAT_COD = $catalogueCode AND GRP_COD = $groupCode AND SGRP_COD = $subGroupCode AND SGS_COD = $sgsCode AND LNG_COD = $languageCode
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);
            sql = @"select SGSMOD_CD,S.MDF_COD,MDF_DSC from SGS_MOD S
                        JOIN MODIF_DSC M ON M.MDF_COD = S.MDF_COD 
                        where S.CAT_COD = $catalogueCode AND GRP_COD = $groupCode AND SGRP_COD = $subGroupCode AND SGS_COD = $sgsCode AND LNG_COD = $languageCode
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);
            sql = @"select O.OPTK_TYPE,O.OPTK_COD,OPTK_DSC from SGS_OPT S
                        JOIN OPTKEYS_DSC O ON O.OPTK_TYPE = S.OPTK_TYPE AND O.OPTK_COD = S.OPTK_COD 
                        where S.CAT_COD = $catalogueCode AND GRP_COD = $groupCode AND SGRP_COD = $subGroupCode AND SGS_COD = $sgsCode AND LNG_COD = $languageCode
                        ";
            AddSgsNarratives(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode, sql, rc);

            return rc;
        }
        private void AddSgsNarratives(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode, string sql, List<string> narratives)
        {
            using (var connection = new SqliteConnection(@"Data Source=C:\Temp\ePerOutput\eperRelease20.db"))
            {
                connection.Open();
                // Variants
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddWithValue("$catalogueCode", catalogueCode);
                command.Parameters.AddWithValue("$groupCode", groupCode);
                command.Parameters.AddWithValue("$subGroupCode", subGroupCode);
                command.Parameters.AddWithValue("$sgsCode", sgsCode);
                command.Parameters.AddWithValue("$languageCode", languageCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        narratives.Add(reader.GetString(0) + reader.GetString(1) + " " + reader.GetString(2));
                    }
                }
            }
        }
    }
}
