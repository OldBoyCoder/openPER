using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class DrawingsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public DrawingsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("Drawings/Detail/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{Scope}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string vin = "", string mvs = "")
        {
            var model = BuildDrawingViewModel(language, makeCode, subMakeCode, modelCode, 
                    catalogueCode, groupCode, subGroupCode, subSubGroupCode, 
                     scope, vin, mvs, _ => drawingNumber);

            return View(model);
        }
        [Route("Detail/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Variant}/{Revision}/{Scope}/{HighlightPart?}")]
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
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode,  string scope,
            string vin, string mvs, Func<List<DrawingKeyViewModel>, int> getCurrentDrawing)
        {
            // Standard prologue
            LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

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
            model.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode,
                modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, vin, mvs);

            return model;
        }


        private List<DrawingKeyViewModel> RemoveUnwantedDrawings(string catalogueCode, string language, List<DrawingKeyViewModel> drawings, string vin, string mvs)
        {
            var rc = new List<DrawingKeyViewModel>();
            string sinComPattern = _rep.GetSincomPattern(mvs);
            string vehiclePattern = _rep.GetVehiclePattern(language, vin);
            var vmkCodes = _rep.GetVmkDataForCatalogue(catalogueCode, language);
            if (vehiclePattern != "") sinComPattern = vehiclePattern;
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
            var tableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.Variant, drawing.Revision, language));
            tableData.MakeCode = drawing.MakeCode;
            tableData.SubMakeCode = drawing.SubMakeCode;
            tableData.ModelCode = drawing.ModelCode;
            tableData.Revision = drawing.Revision;
            tableData.Variant = drawing.Variant;
            if (mvs != "")
            {
                string sinComPattern = _rep.GetSincomPattern(mvs);
                var vmkCodes = _rep.GetVmkDataForCatalogue(drawing.CatalogueCode, language);
                string vehiclePattern = _rep.GetVehiclePattern(language, vin);
                if (vehiclePattern != "") sinComPattern = vehiclePattern;
                var vehicleModificationFilters = _rep.GetFiltersForVehicle(language, vin, mvs);
                foreach (var p in tableData.Parts)
                {
                    var pattern = p.Compatibility;
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        if (!PatternMatchHelper.EvaluateRule(pattern, sinComPattern, vmkCodes, !string.IsNullOrEmpty(vehiclePattern)))
                            p.Visible = false;
                    }
                    var modifications = p.Modifications;
                    foreach (var mod in modifications)
                    {
                        foreach (var rule in mod.Activations)
                        {
                            // Does this apply to this vehicle
                            if (PatternMatchHelper.EvaluateRule(rule.ActivationPattern, sinComPattern, vmkCodes, !string.IsNullOrEmpty(vehiclePattern)))
                            {
                                // Does this vehicle have the data needed
                                if (vehicleModificationFilters.ContainsKey(rule.ActivationCode))
                                {
                                    // Before or after rule?
                                    if (mod.Type == "C")
                                    {
                                        // C means stops at so if data is past this then it is invisible
                                        if (int.Parse(rule.ActivationSpec) <= int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                        {
                                            p.Visible = false;
                                        }
                                    }
                                    if (mod.Type == "D")
                                    {
                                        // C means after a date if data is before this then it is invisible
                                        if (int.Parse(rule.ActivationSpec) > int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                        {
                                            p.Visible = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return tableData;
        }
    }
}
