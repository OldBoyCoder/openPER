using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string VIN = "", string MVS = "")
        {
            // Standard prologue
            LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var model = new DrawingsViewModel();
            model.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, VIN, MVS);

            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings;
            if (scope == "SubSubGroup")
                drawings = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, language);
            else if (scope == "SubGroup")
                drawings = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, language);
            else
                drawings = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode, language);

            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);

            model.Drawings.ForEach(x => x.SubMakeCode = subMakeCode);
            model.Scope = scope;
            // Now we get the rest of the details for the drawing we're interested in
            var drawing = model.Drawings[drawingNumber];
            // Get the table for this drawing
            model.TableData = PopulateTableViewModelFromDrawing(drawing, language, MVS, VIN);
            model.TableData.CurrentDrawing = drawingNumber;

            return View(model);
        }
        [Route("Detail/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Variant}/{Revision}/{Scope}/{HighlightPart?}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int variant, int revision, string scope, string highlightPart = "~", string VIN = "", string MVS = "")
        {
            // Standard prologue
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;


            var model = new DrawingsViewModel();
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings;
            if (scope == "SubSubGroup")
                drawings = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, language);
            else if (scope == "SubGroup")
                drawings = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, language);
            else
                drawings = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode, language);

            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);

            model.Drawings.ForEach(x => x.SubMakeCode = subMakeCode);
            model.Scope = scope;
            // Now we get the rest of the details for the drawing we're interested in
            var drawingNumber = 0;
            foreach (var d in model.Drawings)
            {
                if (d.Variant == variant && d.Revision == revision)
                    break;
                drawingNumber++;
            }
            var drawing = model.Drawings[drawingNumber];
            // Get the table for this drawing
            model.TableData = PopulateTableViewModelFromDrawing(drawing, language, MVS, VIN);
            model.TableData.CurrentDrawing = drawingNumber;
            model.TableData.HighlightPart = highlightPart;

            model.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode,subSubGroupCode, drawingNumber, scope, VIN, MVS);
            return View(model);
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
                foreach (var p in tableData.Parts)
                {
                    var pattern = p.Compatibility;
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        if (!PatternMatchHelper.EvaluateRule(pattern, sinComPattern))
                            p.Visible = false;
                    }
                }
            }

            return tableData;
        }



    }
}
