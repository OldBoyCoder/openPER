﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class DrawingsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        readonly List<SearchEngineViewModel> PartSearchUrl;

        public DrawingsController(IRepository rep, IMapper mapper, IConfiguration config)
        {
            _rep = rep;
            _mapper = mapper;
            PartSearchUrl = NavigationHelper.GetSearchEnginesFromConfig(config);
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("{language}/Drawings/Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{Scope}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string vin = "", string mvs = "")
        {
            var model = BuildDrawingViewModel(language, makeCode, subMakeCode, modelCode,
                    catalogueCode, groupCode, subGroupCode, subSubGroupCode,
                     scope, vin, mvs, _ => drawingNumber);

            return View(model);
        }
        [Route("{language}/Drawings/Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Variant}/{Revision}/{Scope}/{HighlightPart?}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int variant, int revision, string scope, string highlightPart = "~", string vin = "", string mvs = "")
        {
            var model = BuildDrawingViewModel(language, makeCode, subMakeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode,
                scope, vin, mvs, list =>
                {
                    return list.FindIndex(x => x.Variant == variant && x.Revision == revision);
                });
            model.TableData.HighlightPart = highlightPart;

            return View(model);
        }

        private DrawingsViewModel BuildDrawingViewModel(string language, string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string scope,
            string vin, string mvs, Func<List<DrawingKeyViewModel>, int> getCurrentDrawing)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var model = new DrawingsViewModel();

            // We need to get all of the drawing keys for this sub sub group
            var drawings = scope switch
            {
                "SubSubGroup" => _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode,
                    subGroupCode, subSubGroupCode, language),
                "SubGroup" => _rep.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode,
                    subGroupCode, language),
                _ => _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode, language)
            };

            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            if (mvs != "")
                model.Drawings = RemoveUnwantedDrawings(catalogueCode, language, model.Drawings, vin, mvs);

            model.Drawings.ForEach(x => x.SubMakeCode = subMakeCode);
            model.Scope = scope;
            // Now we get the rest of the details for the drawing we're interested in
            var drawingNumber = getCurrentDrawing(model.Drawings);
            // Get the table for this drawing
            model.TableData = PopulateTableViewModelFromDrawing(model.Drawings[drawingNumber], language, mvs, vin);
            model.TableData.CurrentDrawing = drawingNumber;
            model.Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode,
                modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, vin, mvs);
            model.PartSearchUrl = PartSearchUrl;

            return model;
        }


        private List<DrawingKeyViewModel> RemoveUnwantedDrawings(string catalogueCode, string language, List<DrawingKeyViewModel> drawings, string vin, string mvs)
        {
            var rc = new List<DrawingKeyViewModel>();
            var sinComPattern = _rep.GetSincomPattern(mvs);
            var vehiclePattern = _rep.GetVehiclePattern(language, vin);
            var vmkCodes = _rep.GetVmkDataForCatalogue(catalogueCode, language);
            var vehicleModificationFilters = _rep.GetFiltersForVehicle(language, vin, mvs);
            foreach (var d in drawings)
            {
                var pattern = d.VariantPattern;
                var modifications = d.Modifications;
                d.Visible = PatternMatchHelper.ApplyPatternAndModificationRules(pattern, sinComPattern, vmkCodes, vehiclePattern, modifications, vehicleModificationFilters);
                if (d.Visible)
                    rc.Add(d);

            }
            return rc;
        }

        private TableViewModel PopulateTableViewModelFromDrawing(DrawingKeyViewModel drawing, string language, string mvs, string vin)
        {
            var data = _rep.GetTable(drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                drawing.SubSubGroupCode, drawing.Variant, drawing.Revision, language);
            var tableData = _mapper.Map<TableModel, TableViewModel>(data);
            // Hotspots are held at individual part in the table level
            // however we want to unify them so that one tooltip shows all parts for that tooltip
            tableData.HotSpots = new Dictionary<string, PartHotspotViewModel>();
            var options = _rep.GetMvsDetailsForCatalogue(drawing.CatalogueCode, language);

            foreach (var p in tableData.Parts)
            {
                foreach (var h in p.Hotspots)
                {
                    PartHotspotViewModel newHotspot;
                    if (tableData.HotSpots.ContainsKey(h.Key))
                        newHotspot = tableData.HotSpots[h.Key];
                    else
                    {
                        newHotspot = new PartHotspotViewModel
                        {
                            TooltipText = "",
                            X = h.X,
                            Y = h.Y,
                            Width = h.Width,
                            Height = h.Height,
                            TableOrder = p.TableOrder
                        };
                        tableData.HotSpots.Add(h.Key, newHotspot);

                    }
                    newHotspot.TooltipText += $"{p.PartNumber} - {p.FullDescription}<br/>";
                }
                p.CompatibilityTooltip = "";
                if (!string.IsNullOrEmpty(p.Compatibility))
                {
                    var symbols = PatternMatchHelper.GetSymbolsFromPattern(p.Compatibility, out _);
                    foreach (var symbol in symbols.OrderBy(x => x.Key))
                    {
                        var o = options.FirstOrDefault(x => (x.TypeCode + x.ValueCode).Trim() == symbol.Key);
                        if (o != null)
                            p.CompatibilityTooltip += $"<em>{symbol.Key}</em>&nbsp;-&nbsp;{o.TypeDescription} {o.CodeDescription}<br/>";
                        else
                            p.CompatibilityTooltip += $"<em>{symbol.Key}<br/>";
                    }
                }
            }
            //Calculate hotspot as percentages to aid drawing
            foreach (var h in tableData.HotSpots.Values)
            {
                var hFactor = 100.0 / drawing.Width;
                var vFactor = 100.0 / drawing.Height;
                h.XPercent = h.X * hFactor;
                h.YPercent = h.Y * vFactor;
                h.WidthPercent = h.Width * hFactor;
                h.HeightPercent = h.Height * vFactor;
            }
            foreach (var h in tableData.Links)
            {
                var hFactor = 100.0 / drawing.Width;
                var vFactor = 100.0 / drawing.Height;
                h.XPercent = h.X * hFactor;
                h.YPercent = h.Y * vFactor;
                h.WidthPercent = h.Width * hFactor;
                h.HeightPercent = h.Height * vFactor;
                h.LinkDescription = _rep.GetGroupDescription(int.Parse(h.LinkGroupCode), language) + " - " +
                                    _rep.GetSubGroupDescription(int.Parse(h.LinkGroupCode),
                                        int.Parse(h.LinkSubGroupCode), language) +
                                    ((h.LinkSubSubGroupCode != "") ? (
                                    " - " + _rep.GetSubSubGroupDescription(drawing.CatalogueCode, int.Parse(h.LinkGroupCode),
                                        int.Parse(h.LinkSubGroupCode), int.Parse(h.LinkSubSubGroupCode), language)) : "");
            }

            tableData.MakeCode = drawing.MakeCode;
            tableData.SubMakeCode = drawing.SubMakeCode;
            tableData.ModelCode = drawing.ModelCode;
            tableData.Revision = drawing.Revision;
            tableData.Variant = drawing.Variant;
            if (mvs == "") return tableData;

            var sinComPattern = _rep.GetSincomPattern(mvs);
            var vmkCodes
                = _rep.GetVmkDataForCatalogue(drawing.CatalogueCode, language);
            var vehiclePattern = _rep.GetVehiclePattern(language, vin);
            var vehicleModificationFilters = _rep.GetFiltersForVehicle(language, vin, mvs);
            foreach (var p in tableData.Parts)
            {
                var pattern = p.Compatibility;
                if (!string.IsNullOrEmpty(pattern) || p.Modifications.Count > 0)
                {
                    if (!PatternMatchHelper.ApplyPatternAndModificationRules(pattern, sinComPattern, vmkCodes, vehiclePattern,p.Modifications,vehicleModificationFilters))
                        p.Visible = false;
                }

            }
            return tableData;
        }
    }
}
