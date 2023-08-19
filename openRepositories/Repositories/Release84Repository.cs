using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;
using System;
using VinSearcher;

// ReSharper disable StringLiteralTypo

namespace openPERRepositories.Repositories
{
    public class Release84Repository : IRepository
    {
        internal IConfiguration Config;
        internal string PathToDb;
        internal string PathToCdn;
        internal string PathToVindataCh;
        internal string PathToVindataRt;
        public Release84Repository(IConfiguration config)
        {
            Config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s?.FirstOrDefault(x => x.Release == 84);
            if (release == null) return;
            PathToDb = release.DbName;
            PathToCdn = release.CdnRoot;
            PathToVindataCh = release.VinDataCh;
            PathToVindataRt = release.VinDataRt;
        }

        public MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode)
        {
            var model = new MapImageModel();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT MAP_NAME, IMG_NAME
                            FROM CATALOGUES
                            WHERE MK_COD = @p1 AND MK2_COD = @p2 AND CAT_COD = @p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                model.MapName = (reader.IsDBNull(0)) ? "" : reader.GetString(0);
                model.ImageName = PathToCdn + "L_EPERFIG/" + reader.GetString(1);
            }, makeCode, subMakeCode, catalogueCode);
            return model;
        }
        public List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var map = new List<GroupImageMapEntryModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select X1, Y1, NULL, CODE, GRP_DSC from CATALOGUES C
                        JOIN HS_FIGURINI H ON H.IMG_NAME = C.IMG_NAME
                        JOIN GROUPS_DSC GD ON GD.GRP_COD = H.CODE AND LNG_COD = @p2
                        WHERE C.CAT_COD = @p1
                            AND H.CODE IN (SELECT DISTINCT GRP_COD FROM DRAWINGS WHERE CAT_COD = @p1)
                        ORDER BY X1, X2";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new GroupImageMapEntryModel
                {
                    X = reader.GetInt32(0),
                    Y = reader.GetInt32(1),
                    Index = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    GroupCode = int.Parse(reader.GetString(3)),
                    Description = reader.GetString(4)
                };
                map.Add(m);
            }, catalogueCode, languageCode);
            return map;

        }

                public MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
        {
            var map = new MapImageModel();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT MAP_SGRP
                            FROM MAP_VET
                            WHERE CAT_COD = @p1 AND GRP_COD = @p2";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                map.MapName = reader.GetString(0);
            }, catalogue, group);
            if (string.IsNullOrEmpty(map.MapName))
            {
                sql = @"SELECT DISTINCT MAP_NAME
                        FROM MAP_SGRP
                        WHERE GRP_COD = @p1";
                connection.RunSqlFirstRowOnly(sql, (reader) => { map.MapName = reader.GetString(0); }, group);
            }
            sql = @"SELECT DISTINCT IMG_NAME
                        FROM GROUPS
                        WHERE GRP_COD = @p1 AND CAT_COD = @p2";
            connection.RunSqlFirstRowOnly(sql, (reader) => { map.ImageName = PathToCdn + "L_EPERFIG/" + reader.GetString(0); }, group, catalogue);

            return map;
        }

        public List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var map = new List<SubGroupImageMapEntryModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT X1, Y1, S.SGRP_COD, S.SGRP_DSC
                        FROM groups G
                        JOIN subgroups_dsc S ON S.GRP_COD = G.GRP_COD AND S.LNG_COD = @p3
                        JOIN hs_figurini H ON H.IMG_NAME = G.IMG_NAME AND H.CODE = CONCAT(CONVERT(G.GRP_COD, VARCHAR(3)), RIGHT(CONCAT('00',CONVERT(S.SGRP_COD, VARCHAR(2))),2))
                        WHERE G.CAT_COD = @p2 AND G.GRP_COD = @p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubGroupImageMapEntryModel
                {
                    X = reader.GetInt32(0),
                    Y = reader.GetInt32(1),
                    GroupCode = groupCode,
                    SubGroupCode = reader.GetInt32(2),
                    Description = reader.GetString(3)
                };
                map.Add(m);
            }, groupCode, catalogueCode, languageCode);
            return map;
        }

        private (string ImageName, string Hotspots) GetDrawingInfo(string catalogue, int group, int subgroup, int subSubGroup,
            int drawing, int revision)
        {
            (string ImageName, string Hotspots) rc = (null, null);
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT IMG_PATH, HOTSPOTS
                            FROM DRAWINGS
                            WHERE CAT_COD = @p1 AND GRP_COD = @p2 AND SGRP_COD  = @p3 AND SGS_COD = @p4 AND VARIANTE = @p5 AND REVISIONE = @p6";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc.ImageName = reader.GetString(0);
                rc.Hotspots = reader.IsDBNull(1) ? "" : reader.GetString(1);
            }, catalogue, group, subgroup, subSubGroup, drawing, revision);
            return rc;
        }

        public List<MakeModel> GetAllMakes()
        {
            var rc = new List<MakeModel>
            {
                new ("F","F", "FIAT"),
                new ("F","T", "FIAT COMMERCIAL"),
                new ("L","L", "LANCIA"),
                new ("R","R", "ALFA ROMEO"),
                new ("F", "C", "ABARTH")
            };
            return rc;

        }

        public string GetMakeDescription(string makeCode)
        {
            var rc = "";
            var sql = @"SELECT MK_DSC FROM MAKES WHERE MK_COD = @p1";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode);
            return rc;
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<SubSubGroupModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select distinct T.SGS_COD, TD.DSC, PATTERN, MODIF FROM DRAWINGS T
                            JOIN TABLES_DSC TD ON TD.LNG_COD = @p4 AND TD.COD = T.TABLE_DSC_COD
                            WHERE CAT_COD = @p1 AND T.GRP_COD = @p2 AND T.SGRP_COD = @p3
                            order by T.SGS_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubSubGroupModel()
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    Pattern = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Modifications = reader.IsDBNull(3) ? new List<ModificationModel>() :
                        CreateModificationListFromString(catalogueCode, reader.GetString(3), languageCode)
                };

                // Create list of pattern codes.  Very quick and dirty
                var p = m.Pattern;
                p = p.Replace('(', ',').Replace(')', ',').Replace('+', ',').Replace('!', ',');
                var patternParts = p.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var allPatternParts = GetMvsDetailsForCatalogue(catalogueCode, languageCode);
                m.PatternParts = new List<PatternModel>();
                foreach (var part in patternParts)
                {
                    var pattern = new PatternModel
                    {
                        FullCode = part
                    };
                    var d = allPatternParts.FirstOrDefault(x => x.TypeCodePair == part);
                    if (d != null)
                    {
                        pattern.CodeDescription = d.CodeDescription;
                        pattern.TypeDescription = d.TypeDescription;
                    }
                    m.PatternParts.Add(pattern);
                }
                rc.Add(m);
            }, catalogueCode, groupCode, subGroupCode, languageCode);
            return rc;
        }
        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var drawings = new List<DrawingKeyModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC, IMG_PATH, WIDTH, HEIGHT
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = @p5 AND TD.COD = TABLE_DSC_COD
                            WHERE CAT_COD = @p1 AND GRP_COD = @p2 AND SGRP_COD = @p3 AND SGS_COD = @p4
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var drawing = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8),
                    Modifications = reader.IsDBNull(7) ? new List<ModificationModel>() :
                        CreateModificationListFromString(catalogueCode, reader.GetString(7), languageCode),
                    Width = reader.GetInt32(10),
                    Height = reader.GetInt32(11)
                };
                var thumbImagePath = reader.GetString(9);
                var imageParts = thumbImagePath.Split(new[] { '.' });
                drawing.ThumbImagePath = PathToCdn + imageParts[0] + ".th." + imageParts[1];

                drawings.Add(drawing);
            }, catalogueCode, groupCode, subGroupCode, subSubGroupCode, languageCode);

            return drawings;
        }
        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var drawings = new List<DrawingKeyModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC, IMG_PATH, WIDTH, HEIGHT
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = @p4 AND TD.COD = TABLE_DSC_COD
                            WHERE CAT_COD = @p1 AND GRP_COD = @p2 AND SGRP_COD = @p3
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var drawing = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8),
                    Modifications = reader.IsDBNull(7) ? new List<ModificationModel>() :
                        CreateModificationListFromString(catalogueCode, reader.GetString(7), languageCode),
                    Width = reader.GetInt32(10),
                    Height = reader.GetInt32(11),

                };
                var thumbImagePath = reader.GetString(9);
                var imageParts = thumbImagePath.Split(new[] { '.' });
                drawing.ThumbImagePath = PathToCdn + imageParts[0] + ".th." + imageParts[1];

                drawings.Add(drawing);
            }, catalogueCode, groupCode, subGroupCode, languageCode);

            return drawings;
        }
        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var drawings = new List<DrawingKeyModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT CAT_COD, GRP_COD, SGRP_COD, SGS_COD, VARIANTE, IFNULL( PATTERN, ''), REVISIONE, IFNULL(MODIF, ''), DSC, IMG_PATH, WIDTH, HEIGHT
                            FROM DRAWINGS
                            JOIN TABLES_DSC TD ON TD.LNG_COD = @p3 AND TD.COD = TABLE_DSC_COD
                            WHERE CAT_COD = @p1 AND GRP_COD = @p2 
                            ORDER BY GRP_COD, SGRP_COD, SGS_COD, DRW_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var drawing = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = reader.GetString(0),
                    GroupCode = reader.GetInt32(1),
                    SubGroupCode = reader.GetInt32(2),
                    SubSubGroupCode = reader.GetInt32(3),
                    Variant = reader.GetInt32(4),
                    VariantPattern = reader.GetString(5),
                    Revision = reader.GetInt32(6),
                    RevisionModifications = reader.GetString(7),
                    Description = reader.GetString(8),
                    Modifications = reader.IsDBNull(7) ? new List<ModificationModel>() :
                        CreateModificationListFromString(catalogueCode, reader.GetString(7), languageCode),
                    Width = reader.GetInt32(10),
                    Height = reader.GetInt32(11),

                };
                var thumbImagePath = reader.GetString(9);
                var imageParts = thumbImagePath.Split(new[] { '.' });
                drawing.ThumbImagePath = PathToCdn + imageParts[0] + ".th." + imageParts[1];
                drawings.Add(drawing);
            }, catalogueCode, groupCode, languageCode);

            return drawings;
        }
        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var (imageName, hotspots) = GetDrawingInfo(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision);
            var t = new TableModel
            {
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
                SubSubGroupCode = sgsCode,
                GroupDesc = GetGroupDescription(groupCode, languageCode),
                SubGroupDesc = GetSubGroupDescription(groupCode, subGroupCode, languageCode),
                SgsDesc = GetSubSubGroupDescription(catalogueCode, groupCode, subGroupCode, sgsCode, languageCode),
                Parts = GetTableParts(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision, languageCode),
                DrawingNumbers = GetDrawingNumbers(catalogueCode, groupCode, subGroupCode, sgsCode, revision),
                CurrentDrawing = drawingNumber,
                ImagePath = PathToCdn + imageName,
                Links = new List<PartHotspotModel>()
            };
            if (!string.IsNullOrEmpty(hotspots))
            {
                hotspots = hotspots.Replace("L->", "");
                var links = hotspots.Split(";");
                foreach (var link in links)
                {
                    var coords = link.Split(",");
                    if (coords.Length == 5)
                    {
                        var h = new PartHotspotModel
                        {
                            X = int.Parse(coords[1]),
                            Y = int.Parse(coords[2])
                        };
                        h.Width = int.Parse(coords[3]) - h.X + 1;
                        h.Height = int.Parse(coords[4]) - h.Y + 1;
                        h.Link = coords[0];
                        t.Links.Add(h);
                    }
                }
            }
            return t;
        }
        private List<TablePartModel> GetTableParts(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
        {
            var parts = new List<TablePartModel>();
            var sql = @"SELECT TBD_RIF, PRT_COD, TBD_QTY, CDS_DSC, 
                                        TBD_SEQ, NTS_DSC, TBD_VAL_FORMULA, DAD.DSC, MODIF, COL_COD, HOTSPOTS
                                        FROM TBDATA
                                        JOIN CODES_DSC ON TBDATA.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = @p1
                                        LEFT OUTER JOIN NOTES_DSC ON NOTES_DSC.NTS_COD = TBDATA.NTS_COD AND NOTES_DSC.LNG_COD = @p1
                                        LEFT OUTER JOIN DESC_AGG_DSC DAD ON DAD.COD = TBD_AGG_DSC AND DAD.LNG_COD = @p1
                                        WHERE VARIANTE = @p2 AND SGS_COD = @p3 AND SGRP_COD = @p4 AND GRP_COD = @p5 AND CAT_COD = @p6 AND REVISIONE = @p7
                                        ORDER BY TBD_RIF,TBD_SEQ";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var part = new TablePartModel
                {
                    PartNumber = reader.GetString(1),
                    TableOrder = reader.GetInt32(0),
                    Quantity = reader.GetString(2),
                    Description = reader.GetString(3),
                    Sequence = reader.GetInt32(4),
                    Notes = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Compatibility = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    FurtherDescription = reader.IsDBNull(7) ? "" : reader.GetString(7).ToString(),
                    Colour = reader.IsDBNull(9) ? "" : reader.GetString(9).Replace(",", " ")
                };
                if (!reader.IsDBNull(8))
                    part.Modifications = GetPartModifications(catalogueCode, reader.GetString(8), languageCode);
                if (part.Colour != "")
                {
                    part.Colours = CreateColourCollection(catalogueCode, languageCode, part.Colour);
                }

                if (!reader.IsDBNull(10))
                {
                    // We have hotspots
                    var hotspotText = reader.GetString(10);
                    // Hotspots are sets of coordinate x1, y1, x2, y2 separated by semicolons;
                    part.Hotspots = ParseHotspotsFromString(hotspotText, part.TableOrder, part.PartNumber);
                }
                parts.Add(part);

            }, languageCode, drawingNumber, sgsCode, subGroupCode, groupCode, catalogueCode, revision);

            foreach (var part in parts)
            {
                part.IsAComponent = IsPartAComponent(part);
            }
            return parts;
        }

        private static List<PartHotspotModel> ParseHotspotsFromString(string hotspotText, int tableOrder, string partNumber)
        {
            var rc = new List<PartHotspotModel>();
            try
            {
                var sets = hotspotText.Split(";");
                foreach (var set in sets)
                {
                    var coords = set.Split(",");
                    if (coords.Length != 4) break;
                    var h = new PartHotspotModel
                    {
                        X = int.Parse(coords[0]),
                        Y = int.Parse(coords[1])
                    };
                    var x2 = int.Parse(coords[2]);
                    var y2 = int.Parse(coords[3]);
                    h.Width = x2 - h.X + 1;
                    h.Height = y2 - h.Y + 1;
                    h.PartNumber = partNumber;
                    h.TableOrder = tableOrder;
                    rc.Add(h);
                }
            }
            catch (Exception)
            {
                return rc;
            }

            return rc;
        }

        private List<ColourModel> CreateColourCollection(string catalogueCode, string languageCode, string partColour)
        {
            var colours = partColour.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var rc = new List<ColourModel>();
            using var connection = new MySqlConnection(PathToDb);
            foreach (var colour in colours)
            {

                var sql = @"SELECT COL_DSC, IMG_PATH FROM COLOURS_DSC CD 
                        LEFT OUTER JOIN COLOURS_PATH CP ON CD.CAT_COD = CP.CAT_COD AND CD.COL_COD = CP.COL_COD
                        WHERE CD.CAT_COD = @p1 AND CD.LNG_COD = @p2 and CD.COL_COD  = @p3";
                connection.RunSqlAllRows(sql, (reader) =>
                {
                    var c = new ColourModel
                    {
                        Code = colour,
                        Description = reader.GetString(0),
                        ImagePath = reader.IsDBNull(1) ? "" : PathToCdn + "L_EPERTESSUTI/" + reader.GetString(1)
                    };
                    rc.Add(c);

                }, catalogueCode, languageCode, colour);

            }

            return rc;
        }

        private List<ModificationModel> GetPartModifications(string catalogueCode, string modifString, string languageCode)
        {
            var modifications = new List<ModificationModel>();
            if (string.IsNullOrEmpty(modifString)) return modifications;
            var mods = modifString.Split(',');
            var sequence = 1;
            using var connection = new MySqlConnection(PathToDb);

            foreach (var mod in mods)
            {
                var type = mod[..1];
                var code = mod[1..];
                var newMod = new ModificationModel
                {
                    Code = int.Parse(code),
                    Type = type,
                    Progression = sequence++
                };
                var sql = "SELECT MDF_DSC FROM MODIF_DSC WHERE CAT_COD = @p1 AND MDF_COD = @p2 AND LNG_COD = @p3";

                connection.RunSqlFirstRowOnly(sql, (reader) => { newMod.Description = reader.GetString(0); }, catalogueCode, newMod.Code, languageCode);
                modifications.Add(newMod);
            }
            foreach (var mod in modifications)
            {
                mod.Activations = GetActivationsForModification(catalogueCode, mod.Code);
            }
            return modifications;
        }
        private List<ActivationModel> GetActivationsForModification(string catalogueCode, int modCode)
        {
            var modifications = new List<ActivationModel>();
            var sql = @"SELECT IFNULL(ACT_COD, ''),IFNULL(M.MDFACT_SPEC, ''), IFNULL(M.ACT_COD, ''), MDFACT_SPEC, IFNULL(PATTERN, '')
                    FROM MDF_ACT M
                    WHERE M.CAT_COD = @p2 AND M.MDF_COD = @p1";
            using var connection = new MySqlConnection(PathToDb);

            connection.RunSqlAllRows(sql, (reader) =>
            {
                var mod = new ActivationModel
                {
                    ActivationDescription = reader.GetString(0) + " " + reader.GetString(1),
                    ActivationCode = reader.GetString(2),
                    ActivationSpec = reader.GetString(3),
                    ActivationPattern = reader.GetString(4)
                };
                if (mod.ActivationCode == "DAT")
                {
                    if (mod.ActivationSpec.Length == 10)
                    {
                        mod.ActivationSpec = mod.ActivationSpec.Substring(6, 4) + mod.ActivationSpec.Substring(3, 2) + mod.ActivationSpec[..2];

                        modifications.Add(mod);
                    }
                }
                else
                {
                    modifications.Add(mod);
                }

            }, modCode, catalogueCode);
            return modifications;
        }

        public List<ModelModel> GetAllVinModels()
        {
            var rc = new List<ModelModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT CMG_COD, CMG_DSC, MK2_COD FROM COMM_MODGRP ORDER BY CMG_DSC ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new ModelModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = reader.GetString(2)
                };
                rc.Add(m);
            });
            return rc;


        }
        private List<int> GetDrawingNumbers(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int revision)
        {
            var rc = new List<int>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT VARIANTE 
                            FROM DRAWINGS  
                            WHERE SGS_COD = @p1 AND SGRP_COD = @p2 AND GRP_COD = @p3 AND CAT_COD = @p4 AND REVISIONE = @p5";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                rc.Add(reader.GetInt32(0));
            }, sgsCode, subGroupCode, groupCode, catalogueCode, revision);
            return rc;
        }

        private bool IsPartAComponent(TablePartModel part)
        {
            using var connection = new MySqlConnection(PathToDb);
            var rc = false;
            var sql = @"SELECT DISTINCT CPLX_PRT_COD FROM CLICHE WHERE CPLX_PRT_COD = @p1";
            connection.RunSqlFirstRowOnly(sql, (_) =>
            {
                rc = true;

            }, part.PartNumber);
            return rc;
        }
        private string GetCatalogueDescription(string makeCode, string subMakeCode, string catalogueCode)
        {
            string rc = "";
            using var connection = new MySqlConnection(PathToDb);

            var sql = @"SELECT CAT_DSC FROM CATALOGUES WHERE MK_COD = @p1 AND MK2_COD = @p2 AND CAT_COD = @p3";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, makeCode, subMakeCode, catalogueCode);

            return rc;
        }

        public string GetGroupDescription(int groupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            string rc = "";
            var sql = @"SELECT GRP_DSC FROM GROUPS_DSC WHERE GRP_COD = @p1 AND LNG_COD = @p2 ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, languageCode);
            return rc;
        }

        public string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            var rc = "";
            var sql = @"SELECT SGRP_DSC FROM SUBGROUPS_DSC WHERE SGRP_COD = @p2 AND GRP_COD = @p1 AND LNG_COD = @p3 ";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, groupCode, subGroupCode, languageCode);
            return rc;
        }
        public string GetSubSubGroupDescription(string catalogCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            var rc = "";
            var sql = @"select distinct TD.DSC FROM DRAWINGS T
                            JOIN TABLES_DSC TD ON TD.LNG_COD = @p5 AND TD.COD = T.TABLE_DSC_COD
                            WHERE CAT_COD = @p1 AND T.GRP_COD = @p2 AND T.SGRP_COD = @p3 AND T.SGS_COD = @p4
                            ";

            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catalogCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            return rc;
        }


        private string GetModelDescription(string subMakeCode, string modelCode)
        {
            var rc = "";
            var sql = @"SELECT CMG_DSC FROM COMM_MODGRP WHERE MK2_COD = @p1 AND CMG_COD = @p2";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, subMakeCode, modelCode);
            return rc;
        }

        public List<ModelModel> GetAllModelsForMake(string makeCode, string subMake)
        {
            var rc = new List<ModelModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT CMG_COD, CMG_DSC FROM COMM_MODGRP WHERE MK2_COD = @p1  ORDER BY CMG_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                var m = new ModelModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = makeCode,
                    SubMakeCode = subMake,
                    ImagePath = PathToCdn + $"SmallModelImages{subMake}/{reader.GetString(0).ToUpper()}.jpg"

                };
                rc.Add(m);
            }, subMake);
            return rc;
        }

        public List<CatalogueModel> GetAllCatalogues(string makeCode, string subMakeCode, string modelCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<CatalogueModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT CAT_COD, CAT_DSC FROM CATALOGUES WHERE MK2_COD = @p2 AND MK_COD =@p1 AND CMG_COD = @p3  ORDER BY CAT_SORT_KEY ";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new CatalogueModel
                {
                    Code = reader.GetString(0),
                    Description = reader.GetString(1),
                    MakeCode = makeCode,
                    SubMakeCode = subMakeCode,
                    ModelCode = modelCode
                };
                rc.Add(m);
            }, makeCode, subMakeCode, modelCode);
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<GroupModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select distinct T.GRP_COD, GRP_DSC FROM DRAWINGS T
                            JOIN GROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.LNG_COD = @p2
                            WHERE CAT_COD = @p1
                            order by T.GRP_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new GroupModel
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1)
                };
                rc.Add(m);
            }, catalogueCode, languageCode);
            //foreach (var group in rc)
            //{
            //    group.SubSubGroups = GetSubgroupsForCatalogueGroup(catalogueCode, group.Code, languageCode);
            //}
            return rc;
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<SubGroupModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select distinct T.SGRP_COD, SGRP_DSC FROM DRAWINGS T
                            JOIN SUBGROUPS_DSC G ON G.GRP_COD = T.GRP_COD AND G.SGRP_COD = T.SGRP_COD AND G.LNG_COD = @p1
                            WHERE CAT_COD = @p2 AND T.GRP_COD = @p3
                            order by T.SGRP_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new SubGroupModel
                {
                    Code = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    GroupCode = groupCode
                };
                rc.Add(m);
            }, languageCode, catalogueCode, groupCode);
            return rc;
        }
        private List<ModificationModel> CreateModificationListFromString(string catalogueCode, string modificationList, string languageCode)
        {
            var modList = new List<ModificationModel>();
            if (string.IsNullOrEmpty(modificationList)) return modList;
            var parts = modificationList.Split(',');
            var sequence = 1;
            foreach (var part in parts)
            {
                var mod = new ModificationModel
                {
                    Type = part.Substring(0, 1),
                    Code = int.Parse(part.Substring(1)),
                    Progression = sequence++
                };
                var sql = "SELECT MDF_DSC FROM MODIF_DSC WHERE CAT_COD = @p1 AND MDF_COD = @p2 AND LNG_COD = @p3";
                using var connection = new MySqlConnection(PathToDb);
                connection.RunSqlFirstRowOnly(sql, (reader) =>
                {
                    mod.Description = reader.GetString(0);
                }, catalogueCode, mod.Code, languageCode);
                mod.Activations = GetActivationsForModification(catalogueCode, mod.Code);
                modList.Add(mod);
            }
            return modList;
        }


        public List<LanguageModel> GetAllLanguages()
        {
            var languages = new List<LanguageModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT LNG_COD, LNG_DSC FROM LANG ORDER BY LNG_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var language = new LanguageModel
                {
                    Code = openPERHelpers.LanguageSupport.GetIso639CodeFromString(reader.GetString(0)),
                    Description = reader.GetString(1)
                };
                languages.Add(language);
            });
            return languages;
        }
        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            breadcrumb.Language = languageCode;
            if (breadcrumb.MakeCode != null) breadcrumb.MakeDescription = GetMakeDescription(breadcrumb.MakeCode);
            if (breadcrumb.SubMakeCode != null) breadcrumb.SubMakeDescription = GetSubMakeDescription(breadcrumb.SubMakeCode);
            if (breadcrumb.ModelCode != null) breadcrumb.ModelDescription = GetModelDescription(breadcrumb.SubMakeCode, breadcrumb.ModelCode);

            if (breadcrumb.CatalogueCode != null) breadcrumb.CatalogueDescription = GetCatalogueDescription(breadcrumb.MakeCode, breadcrumb.SubMakeCode,
                breadcrumb.CatalogueCode);
            if (breadcrumb.GroupCode != null) breadcrumb.GroupDescription = GetGroupDescription(breadcrumb.GroupCode.Value, languageCode);
            if (breadcrumb.GroupCode != null && breadcrumb.SubGroupCode != null) breadcrumb.SubGroupDescription = GetSubGroupDescription(breadcrumb.GroupCode.Value, breadcrumb.SubGroupCode.Value, languageCode);
            if (breadcrumb.GroupCode != null && breadcrumb.SubGroupCode != null && breadcrumb.SubSubGroupCode != null) breadcrumb.SubSubGroupDescription = GetSubSubGroupDescription(breadcrumb.CatalogueCode, breadcrumb.GroupCode.Value, breadcrumb.SubGroupCode.Value, breadcrumb.SubSubGroupCode.Value, languageCode);
        }
        public List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, string clichePartNumber)
        {
            var drawings = new List<DrawingKeyModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT DISTINCT CPD_NUM, CLH_COD, IMG_PATH
                            FROM CLICHE
                            WHERE CPLX_PRT_COD = @p1
                            ORDER BY CPD_NUM";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var drawing = new DrawingKeyModel()
                {
                    MakeCode = makeCode,
                    ModelCode = modelCode,
                    CatalogueCode = catalogueCode,
                    GroupCode = groupCode,
                    SubGroupCode = subGroupCode,
                    SubSubGroupCode = subSubGroupCode,
                    ClichePartNumber = clichePartNumber,
                    ClichePartDrawingNumber = reader.GetInt32(0),
                    ClichePartCode = reader.GetString(1)
                };
                var thumbImagePath = reader.GetString(2);
                var imageParts = thumbImagePath.Split(new[] { '.' });
                drawing.ThumbImagePath = PathToCdn + imageParts[0] + ".th." + imageParts[1];
                drawing.ImagePath = PathToCdn + thumbImagePath;

                drawings.Add(drawing);
            }, clichePartNumber);
            return drawings;
        }

        public List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber,
            int clicheDrawingNumber, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            var rc = new List<TablePartModel>();
            var sql = @"SELECT C.PRT_COD, CLH_COD, CPD_RIF, CPD_QTY, IFNULL(DAD.DSC, ''), CDS_DSC
                        FROM CPXDATA C 
                        JOIN PARTS P ON P.PRT_COD = C.PRT_COD
                        JOIN CODES_DSC ON P.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = @p3
                        LEFT OUTER JOIN DESC_AGG_DSC DAD ON DAD.COD = C.CPD_AGG_DSC AND DAD.LNG_COD = @p3
                        WHERE C.CPLX_PRT_COD = @p1 AND C.CPD_NUM = @p2
                        ORDER BY CPD_RIF, CPD_RIF_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                rc.Add(new TablePartModel
                {
                    PartNumber = reader.GetString(0),
                    TableOrder = reader.GetInt32(2),
                    Quantity = reader.GetString(3),
                    FurtherDescription = reader.GetString(4),
                    Description = reader.GetString(5)
                });
            }, clichePartNumber, clicheDrawingNumber, languageCode);

            var maxRif = 1;
            if (rc.Count > 0)
                maxRif = rc.Max(x => x.TableOrder) + 1;
            // Now get any kits
            sql = @"SELECT K.PRT_COD, CDS_DSC
                        FROM KIT K
                        JOIN PARTS P ON P.PRT_COD = K.PRT_COD
                        JOIN CODES_DSC ON P.CDS_COD = CODES_DSC.CDS_COD AND CODES_DSC.LNG_COD = @p3
                        WHERE K.CPLX_PRT_COD = @p1 AND K.CAT_COD = @p2
                        ORDER BY TBD_SEQ";
            connection.RunSqlAllRows(sql, (reader) =>
            {

                rc.Add(new TablePartModel
                {
                    PartNumber = reader.GetString(0),
                    TableOrder = maxRif++,
                    Quantity = "01",
                    FurtherDescription = "",
                    Description = reader.GetString(1)
                });
            }, clichePartNumber, catalogueCode, languageCode);


            return rc;
        }
        private string GetSubMakeDescription(string subMakeCode)
        {
            var allMakes = GetAllMakes();
            return allMakes.FirstOrDefault(x => x.SubCode == subMakeCode)?.Description;
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            PartModel p = null;
            using (var connection = new MySqlConnection(PathToDb))
            {
                var sql = @"select P.PRT_COD, C.CDS_COD, C.CDS_DSC,F.FAM_COD, F.FAM_DSC, U.UM_COD, U.UM_DSC, PRT_WEIGHT  from PARTS P 
                                JOIN CODES_DSC C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = @p2
                                JOIN FAM_DSC F ON F.FAM_COD = P.PRT_FAM_COD AND F.LNG_COD = @p2
                                LEFT OUTER  JOIN UN_OF_MEAS U ON U.UM_COD = P.UM_COD
                                LEFT OUTER JOIN RPLNT R ON R.RPL_COD = P.PRT_COD
                                where P.PRT_COD = @p1";
                connection.RunSqlFirstRowOnly(sql, (reader) =>
                {
                    p = new PartModel
                    {
                        PartNumber = reader.GetString(0),
                        Description = reader.GetString(2),
                        FamilyCode = reader.GetString(3),
                        FamilyDescription = reader.GetString(4),
                        UnitOfSale = reader.GetString(5) + " " + reader.GetString(6),
                        Weight = reader.GetInt32(7)
                    };
                }, partNumberSearch, languageCode);

            }
            if (p != null)
            {
                p.Drawings = GetDrawingsForPartNumber(p.PartNumber, languageCode);
                p.ReplacedBy = GetReplacedByForPartNumber(p.PartNumber, languageCode);
                p.Replaces = GetReplacesForPartNumber(p.PartNumber, languageCode);
            }
            return p;
        }

        private List<PartReplacementModel> GetReplacedByForPartNumber(string partNumber, string languageCode)
        {
            var rc = new List<PartReplacementModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT RPL_DATE, RPL_COD, RPL_GRPDSC, C.CDS_DSC
                                 FROM rplnt R
                                 JOIN parts P ON R.RPL_COD = P.PRT_COD
                                 JOIN codes_dsc C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = @p2
                                 LEFT OUTER  JOIN rplnt_grp G ON G.RPL_GRPNUM = R.RPL_GRPNUM AND G.LNG_COD = @p2 AND G.PRT_COD = R.PRT_COD
                                 WHERE R.prt_cod = @p1 ORDER BY R.RPL_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartReplacementModel
                {
                    ReplacementDate = reader.GetString(0),
                    NewPartCode = reader.GetString(1),
                    OldPartCode = partNumber,
                    GroupDescription = reader.IsDBNull(2) ? "" : reader.GetString(3),
                    PartDescription = reader.GetString(3)
                };
                rc.Add(p);
            }, partNumber, languageCode);
            return rc;
        }
        private List<PartReplacementModel> GetReplacesForPartNumber(string partNumber, string languageCode)
        {
            var rc = new List<PartReplacementModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT RPL_DATE, R.PRT_COD, RPL_GRPDSC, C.CDS_DSC
                                 FROM rplnt R
                                 JOIN parts P ON R.RPL_COD = P.PRT_COD
                                 JOIN codes_dsc C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = @p2
                                 LEFT OUTER  JOIN rplnt_grp G ON G.RPL_GRPNUM = R.RPL_GRPNUM AND G.LNG_COD = @p2 AND G.PRT_COD = R.PRT_COD
                                 WHERE R.RPL_COD = @p1 ORDER BY R.RPL_COD";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartReplacementModel
                {
                    ReplacementDate = reader.GetString(0),
                    OldPartCode = reader.GetString(1),
                    NewPartCode = partNumber,
                    GroupDescription = reader.IsDBNull(2) ? "" : reader.GetString(3),
                    PartDescription = reader.GetString(3)
                };
                rc.Add(p);
            }, partNumber, languageCode);
            return rc;
        }

        public List<PartModel> GetPartSearch(string modelName, string partDescription, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<PartModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select P.PRT_COD, C.CDS_COD, C.CDS_DSC,F.FAM_COD, F.FAM_DSC, U.UM_COD, U.UM_DSC, PRT_WEIGHT  from PARTS P 
                                JOIN CODES_DSC C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = @p1
                                JOIN FAM_DSC F ON F.FAM_COD = P.PRT_FAM_COD AND F.LNG_COD = @p1
                                JOIN APPLICABILITY A ON A.PRT_COD = P.PRT_COD 
                                JOIN catalogues CT ON CT.CAT_COD = A.CAT_COD
                                LEFT OUTER  JOIN UN_OF_MEAS U ON U.UM_COD = P.UM_COD
                                LEFT OUTER JOIN RPLNT R ON R.RPL_COD = P.PRT_COD
                                where C.CDS_DSC LIKE '%" + partDescription + "%' AND CT.CAT_DSC LIKE '%" + modelName + "%'";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartModel
                {
                    PartNumber = reader.GetString(0),
                    Description = reader.GetString(2),
                    FamilyCode = reader.GetString(3),
                    FamilyDescription = reader.GetString(4),
                    UnitOfSale = reader.GetString(6),
                    Weight = reader.GetInt32(7)
                };
                p.Drawings = GetDrawingsForPartNumber(p.PartNumber, languageCode);
                rc.Add(p);
            }, languageCode);
            return rc;
        }
        public List<PartModel> GetBasicPartSearch(string modelName, string partDescription, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<PartModel>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select DISTINCT P.PRT_COD, C.CDS_COD, C.CDS_DSC,F.FAM_COD, F.FAM_DSC, CT.CAT_DSC, CT.CAT_COD  from PARTS P 
                                JOIN CODES_DSC C ON C.CDS_COD = P.CDS_COD AND C.LNG_COD = @p1
                                JOIN FAM_DSC F ON F.FAM_COD = P.PRT_FAM_COD AND F.LNG_COD = @p1
                                JOIN APPLICABILITY A ON A.PRT_COD = P.PRT_COD 
                                JOIN catalogues CT ON CT.CAT_COD = A.CAT_COD
                                LEFT OUTER  JOIN UN_OF_MEAS U ON U.UM_COD = P.UM_COD
                                where C.CDS_DSC LIKE '%" + partDescription + "%' AND CT.CAT_DSC LIKE '%" + modelName + "%'";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartModel
                {
                    CatalogueDescription = reader.GetString(5),
                    CatalogueCode = reader.GetString(6),
                    PartNumber = reader.GetString(0),
                    Description = reader.GetString(2),
                    FamilyCode = reader.GetString(3),
                    FamilyDescription = reader.GetString(4)
                };
                p.Drawings = GetDrawingsForPartNumber(p.CatalogueCode, p.PartNumber, languageCode);
                rc.Add(p);
            }, languageCode);
            return rc;
        }


        private List<PartDrawing> GetDrawingsForPartNumber(string partNumber, string languageCode)
        {
            var drawings = new List<PartDrawing>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select A.CAT_COD, C.CAT_DSC, A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, MK_COD, CMG_COD, MK2_COD, REVISIONE  
                            FROM APPLICABILITY A
							JOIN CATALOGUES C ON C.CAT_COD = A.CAT_COD
							JOIN TBDATA T ON T.PRT_COD = A.PRT_COD AND T.CAT_COD = A.CAT_COD AND T.GRP_COD = A.GRP_COD AND T.SGRP_COD = A.SGRP_COD
							JOIN SUBGROUPS_DSC SD ON SD.SGRP_COD = T.SGRP_COD AND SD.GRP_COD = T.GRP_COD AND SD.LNG_COD = @p2
							where A.prt_cod = @p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    Revision = reader.GetInt32(10),
                    ClichePart = false
                };
                drawings.Add(p);
            }, partNumber, languageCode);

            // This could be a part in a cliche
            sql = @"select DISTINCT 
	                A.CAT_COD, CT.CAT_DSC , A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, CT.MK_COD, CMG_COD, MK2_COD,
	                CDS.CDS_DSC, T.PRT_COD, C.PRT_COD, C.CPD_NUM, T.REVISIONE
                        from CPXDATA C
                        JOIN APPLICABILITY A ON C.PRT_COD = A.PRT_COD
                        JOIN CATALOGUES CT ON CT.CAT_COD = A.CAT_COD
                        JOIN CODES_DSC CDS ON CDS.CDS_COD = A.CDS_COD AND CDS.LNG_COD = @p2
                        JOIN TBDATA T ON T.PRT_COD = C.CPLX_PRT_COD AND T.CAT_COD = A.CAT_COD
	                        AND A.GRP_COD = T.GRP_COD AND A.SGRP_COD = T.SGRP_COD
                        JOIN SUBGROUPS_DSC SD ON SD.GRP_COD = T.GRP_COD AND SD.SGRP_COD = T.SGRP_COD AND SD.LNG_COD = @p2

                        WHERE C.PRT_COD = @p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    ClichePartNumber = reader.GetString(11),
                    ClichePartDrawingNumber = reader.GetInt32(13),
                    Revision = reader.GetInt32(14),
                    ClichePart = true
                };
                drawings.Add(p);
            }, partNumber, languageCode);

            return drawings;
        }
        private List<PartDrawing> GetDrawingsForPartNumber(string catalogueCode, string partNumber, string languageCode)
        {
            var drawings = new List<PartDrawing>();
            using var connection = new MySqlConnection(PathToDb);
            var sql = @"select A.CAT_COD, C.CAT_DSC, A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, MK_COD, CMG_COD, MK2_COD, REVISIONE  
                            FROM APPLICABILITY A
							JOIN CATALOGUES C ON C.CAT_COD = A.CAT_COD
							JOIN TBDATA T ON T.PRT_COD = A.PRT_COD AND T.CAT_COD = A.CAT_COD AND T.GRP_COD = A.GRP_COD AND T.SGRP_COD = A.SGRP_COD
							JOIN SUBGROUPS_DSC SD ON SD.SGRP_COD = T.SGRP_COD AND SD.GRP_COD = T.GRP_COD AND SD.LNG_COD = @p2
							where A.prt_cod = @p1 AND A.CAT_COD = @p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    Revision = reader.GetInt32(10),
                    ClichePart = false
                };
                drawings.Add(p);
            }, partNumber, languageCode, catalogueCode);

            // This could be a part in a cliche
            sql = @"select DISTINCT 
	                A.CAT_COD, CT.CAT_DSC , A.GRP_COD, A.SGRP_COD, T.SGS_COD, T.VARIANTE, SGRP_DSC, CT.MK_COD, CMG_COD, MK2_COD,
	                CDS.CDS_DSC, T.PRT_COD, C.PRT_COD, C.CPD_NUM, T.REVISIONE
                        from CPXDATA C
                        JOIN APPLICABILITY A ON C.PRT_COD = A.PRT_COD
                        JOIN CATALOGUES CT ON CT.CAT_COD = A.CAT_COD
                        JOIN CODES_DSC CDS ON CDS.CDS_COD = A.CDS_COD AND CDS.LNG_COD = @p2
                        JOIN TBDATA T ON T.PRT_COD = C.CPLX_PRT_COD AND T.CAT_COD = A.CAT_COD
	                        AND A.GRP_COD = T.GRP_COD AND A.SGRP_COD = T.SGRP_COD
                        JOIN SUBGROUPS_DSC SD ON SD.GRP_COD = T.GRP_COD AND SD.SGRP_COD = T.SGRP_COD AND SD.LNG_COD = @p2

                        WHERE C.PRT_COD = @p1 AND A.CAT_COD = @p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new PartDrawing
                {
                    Make = reader.GetString(7),
                    SubMake = reader.GetString(9),
                    Model = reader.GetString(8),
                    CatalogueCode = reader.GetString(0),
                    CatalogueDescription = reader.GetString(1),
                    GroupCode = reader.GetInt32(2),
                    SubGroupCode = reader.GetInt32(3),
                    SubSubGroupCode = reader.GetInt32(4),
                    Variant = reader.GetInt32(5),
                    SubGroupDescription = reader.GetString(6),
                    ClichePartNumber = reader.GetString(11),
                    ClichePartDrawingNumber = reader.GetInt32(13),
                    Revision = reader.GetInt32(14),
                    ClichePart = true
                };
                drawings.Add(p);
            }, partNumber, languageCode, catalogueCode);

            return drawings;
        }

        private List<MvsDataModel> GetMvsDataWithVagueMvsCode(string mvsMarque, string mvsModel, string mvsVersion)
        {
            using var connection = new MySqlConnection(PathToDb);
            var rc = new List<MvsDataModel>();
            var sql = @"SELECT M.CAT_COD, MVS_DSC, VMK_TYPE_M, VMK_COD_M, VMK_TYPE_V, VMK_COD_V, VMK_TYPE_R, 
                            VMK_COD_R, MVS_ENGINE_TYPE, MVS_DOORS_NUM, SINCOM, PATTERN,
                            C.CAT_DSC, C.MK_COD, C.MK2_COD,
                            MD.CMG_DSC, C.CMG_COD
                            FROM MVS M
                            JOIN CATALOGUES C ON C.CAT_COD = M.CAT_COD
                            JOIN COMM_MODGRP MD ON MD.MK2_COD = C.MK2_COD AND MD.CMG_COD = C.CMG_COD
                            WHERE MOD_COD = @p1 AND MVS_VERSION = @p2 AND MVS_SERIE = @p3";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new MvsDataModel
                {
                    MvsMark = mvsMarque,
                    MvsModel = mvsModel,
                    MvsVersion = mvsVersion,
                    CatalogueCode = reader.GetString(0),
                    Description = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    EngineType = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    Sincom = reader.IsDBNull(10) ? "" : reader.GetString(10),
                    CatalogueDescription = reader.IsDBNull(12) ? "" : reader.GetString(12),
                    MakeCode = reader.IsDBNull(13) ? "" : reader.GetString(13),
                    SubMakeCode = reader.IsDBNull(14) ? "" : reader.GetString(14),
                    ModelDescription = reader.IsDBNull(15) ? "" : reader.GetString(15),
                    ModelCode = reader.IsDBNull(16) ? "" : reader.GetString(16),
                    Pattern = reader.IsDBNull(11) ? "" : reader.GetString(11)
                };
                rc.Add(p);
            }, mvsMarque, mvsModel, mvsVersion);

            return rc;

        }

        public string GetInteriorColourDescription(string catCode, string interiorColourCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            var rc = "Unknown";
            var sql = @"SELECT DSC_COLORE_INT_VET
                            FROM INTERNAL_COLOURS_DSC
                            WHERE CAT_COD = @p1 AND COD_COLORE_INT_VET = @p2 AND LNG_COD = @p3";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catCode, interiorColourCode, languageCode);

            return rc;
        }
        public string GetExteriorColourDescription(string catCode, string exteriorColourCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            using var connection = new MySqlConnection(PathToDb);
            var rc = "Unknown";
            var sql = @"SELECT DSC_COLORE_EXT_VET
                            FROM EXTERNAL_COLOURS_DSC
                            WHERE CAT_COD = @p1 AND COD_COLORE_EXT_VET = @p2 AND LNG_COD = @p3";
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.GetString(0);
            }, catCode, exteriorColourCode, languageCode);

            return rc;
        }

        public string GetImageNameForModel(string makeCode, string subMakeCode, string modelCode)
        {
            return PathToCdn + $"ModelImages{subMakeCode}/{modelCode.ToUpper()}.jpg";
        }

        public List<VinSearchResultModel> FindMatchesForVin(string languageCode, string fullVin)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<VinSearchResultModel>();
            var x = new Release84VinSearch(PathToVindataCh, PathToVindataRt);

            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return null;

            using var connection = new MySqlConnection(PathToDb);
            var sql = @"SELECT MOD_COD FROM VIN WHERE VIN_COD = @p1";
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var modelCode = reader.GetString(0);
                var searchResult = x.FindVehicleByModelAndChassis(modelCode, fullVin.Substring(9, 8));
                if (searchResult != null)
                {
                    var v = new VinSearchResultModel
                    {
                        BuildDate = searchResult.Date,
                        Chassis = searchResult.Chassis,
                        Motor = searchResult.Motor,
                        Caratt = searchResult.Caratt,
                        InteriorColourCode = searchResult.InteriorColour,
                        Vin = searchResult.Vin,
                        Mvs = searchResult.Mvs,
                        Organization = searchResult.Organization
                    };
                    if (v.Vin == "") v.Vin = fullVin;
                    rc.Add(v);
                }

            }, fullVin[3..6]);

            return rc;
        }
        public List<VinSearchResultModel> FindMatchesForMvsAndVin(string languageCode, string mvs, string fullVin)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);

            var rc = new List<VinSearchResultModel>();
            var x = new Release84VinSearch(PathToVindataCh, PathToVindataRt);

            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return null;

            var modelCode = mvs.Substring(0, 3);
            var searchResult = x.FindVehicleByModelAndChassis(modelCode, fullVin.Substring(9, 8));
            if (searchResult != null)
            {
                var v = new VinSearchResultModel
                {
                    BuildDate = searchResult.Date,
                    Chassis = searchResult.Chassis,
                    Motor = searchResult.Motor,
                    Caratt = searchResult.Caratt,
                    InteriorColourCode = searchResult.InteriorColour,
                    Vin = searchResult.Vin,
                    Mvs = searchResult.Mvs,
                    Organization = searchResult.Organization
                };
                if (!string.IsNullOrEmpty(v.Caratt))
                    v.Caratt = v.Caratt.Replace("|", "");
                if (v.Vin == "") v.Vin = fullVin;
                rc.Add(v);
            }


            return rc;
        }

        public List<MvsDataModel> GetMvsDetails(string mvs)
        {
            return GetMvsDataWithVagueMvsCode(mvs[..3], mvs.Substring(3, 3), mvs.Substring(6, 1));
        }
        public List<MvsCatalogueOptionModel> GetMvsDetailsForCatalogue(string catalogueCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<MvsCatalogueOptionModel>();
            var sql = @"SELECT CONCAT( IFNULL(V.VMK_TYPE, ''),IFNULL(VMK_COD, '')), C.vmk_dsc, V.VMK_DSC, V.VMK_TYPE, V.VMK_COD FROM VMK_DSC V
	                        LEFT OUTER JOIN carat_dsc C ON C.CAT_COD = V.CAT_COD AND C.LNG_COD = @p2 AND V.VMK_TYPE = C.VMK_TYPE
	                        WHERE V.CAT_COD = @p1 AND V.LNG_COD = @p2 
                            ORDER BY C.VMK_TYPE, VMK_COD";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var p = new MvsCatalogueOptionModel
                {
                    TypeCodePair = reader.GetString(0),
                    TypeDescription = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    CodeDescription = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    TypeCode = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    ValueCode = reader.IsDBNull(4) ? "" : reader.GetString(4)
                };
                rc.Add(p);

            }, catalogueCode, languageCode);
            return rc;
        }

        public string GetSincomPattern(string mVs)
        {
            var rc = "";
            var sql = @"SELECT PATTERN FROM MVS WHERE SINCOM = @p1";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlFirstRowOnly(sql, (reader) =>
            {
                rc = reader.IsDBNull(0) ? "" : reader.GetString(0);

            }, mVs);
            return rc;
        }

        public Dictionary<string, string> GetFiltersForVehicle(string languageCode, string vin, string mvs)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var vehicles = FindMatchesForVin(languageCode, vin);
            var rc = new Dictionary<string, string>();
            if (vehicles == null || vehicles.Count == 0) return rc;
            var v = vehicles[0];
            if (!string.IsNullOrEmpty(v.Chassis)) rc.Add("TEL", $"{v.Chassis}");
            if (!string.IsNullOrEmpty(v.Motor)) rc.Add("MOT", $"{v.Motor}");
            if (!string.IsNullOrEmpty(v.BuildDate)) rc.Add("DAT", $"{v.BuildDate}");
            if (!string.IsNullOrEmpty(v.Organization)) rc.Add("VET", $"{v.Organization}");
            return rc;
        }

        public string GetVehiclePattern(string languageCode, string vin)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = "";
            if (string.IsNullOrEmpty(vin)) return "";
            var vehicles = FindMatchesForVin(languageCode, vin);
            if (vehicles.Count == 0) return "";
            foreach (var v in vehicles)
            {
                if (v.Vin == vin)
                {
                    return v.Caratt ?? "";
                }
            }
            return rc;
        }

        public List<VmkModel> GetVmkDataForCatalogue(string catalogueCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<VmkModel>();
            var sql = @"SELECT VD.VMK_TYPE, VD.VMK_COD, VD.VMK_DSC, CD.VMK_DSC FROM VMK_DSC VD
                LEFT OUTER JOIN CARAT_DSC CD ON CD.CAT_COD = @p1 AND CD.VMK_TYPE = VD.VMK_TYPE AND CD.LNG_COD = @p2
                    WHERE VD.CAT_COD = @p1 and VD.LNG_COD =@p2";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var v = new VmkModel
                {
                    Type = reader.GetString(0),
                    Code = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    CodeDescription = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    TypeDescription = reader.IsDBNull(3) ? "" : reader.GetString(3)
                };
                v.MultiValue = !string.IsNullOrEmpty(v.Code);
                rc.Add(v);

            }, catalogueCode, languageCode);
            return rc;
        }
        public List<CatalogueVariantsModel> GetCatalogueVariants(string catalogueCode)
        {
            var rc = new List<CatalogueVariantsModel>();
            var sql = @"SELECT MVS_DSC, SINCOM, Pattern FROM MVS WHERE CAT_COD = @p1";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var v = new CatalogueVariantsModel
                {
                    Description = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    Sincom = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Pattern = reader.IsDBNull(2) ? "" : reader.GetString(2)
                };
                rc.Add(v);

            }, catalogueCode);
            return rc;
        }

        public List<ModificationModel> GetCatalogueModifications(string catalogueCode, string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = new List<ModificationModel>();
            var sql = @"SELECT MDF_COD, MDF_DSC FROM MODIF_DSC WHERE CAT_COD = @p1 AND LNG_COD = @p2";
            using var connection = new MySqlConnection(PathToDb);
            connection.RunSqlAllRows(sql, (reader) =>
            {
                var m = new ModificationModel()
                {
                    Code = reader.GetInt32(0),
                    Description = reader.IsDBNull(1) ? "" : reader.GetString(1),
                };
                m.Activations = GetActivationsForModification(catalogueCode, m.Code);
                rc.Add(m);

            }, catalogueCode, languageCode);
            return rc;
        }

        public List<MakeModel> GetCatalogueHierarchy(string languageCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);

            var rc = GetAllMakes();
            foreach (var make in rc)
            {
                make.Models = GetAllModelsForMake(make.Code, make.SubCode);
                foreach (var model in make.Models)
                {
                    model.Catalogues = GetAllCatalogues(make.Code, make.SubCode, model.Code, languageCode);
                    //foreach (var cat in model.Catalogues)
                    //{
                    //    cat.Groups = GetGroupsForCatalogue(cat.Code, languageCode);
                    //    foreach (var group in cat.Groups)
                    //    {
                    //        group.SubGroups = GetSubGroupsForCatalogueGroup(cat.Code, group.Code, languageCode);
                    //    }
                    //}
                }
            }
            return rc;
        }
        public List<GroupModel> GetAllSectionsForCatalogue(string languageCode, string catalogueCode)
        {
            languageCode = openPERHelpers.LanguageSupport.GetFiatLanguageCodeFromString(languageCode);
            var rc = GetGroupsForCatalogue(catalogueCode, languageCode);
            foreach (var group in rc)
            {
                group.SubGroups = GetSubGroupsForCatalogueGroup(catalogueCode, group.Code, languageCode);
            }
            return rc;
        }
    }
}
