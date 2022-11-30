using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        [Route("Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{Scope}")]
        public IActionResult Detail(string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

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
            var drawing = model.Drawings[drawingNumber];
            // Get the table for this drawing
            model.TableData = PopulateTableViewModelFromDrawing(drawing, language);
            model.TableData.CurrentDrawing = drawingNumber;
            // Sort out breadcrumbs
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = drawing.MakeCode,
                SubMakeCode = drawing.SubMakeCode,
                ModelCode = drawing.ModelCode,
                CatalogueCode = drawing.CatalogueCode,
                GroupCode = drawing.GroupCode,
                SubGroupCode = drawing.SubGroupCode,
                SubSubGroupCode = drawing.SubSubGroupCode,
                DrawingNumber = drawingNumber
            };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);
            model.Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb);

            return View(model);
        }
        [Route("Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Variant}/{Revision}/{Scope}/{HighlightPart?}")]
        public IActionResult Detail(string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int variant, int revision, string scope, string highlightPart = "~")
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

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
            model.TableData = PopulateTableViewModelFromDrawing(drawing, language);
            model.TableData.CurrentDrawing = drawingNumber;
            model.TableData.HighlightPart = highlightPart;
            // Sort out breadcrumbs
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = drawing.MakeCode,
                SubMakeCode = drawing.SubMakeCode,
                ModelCode = drawing.ModelCode,
                CatalogueCode = drawing.CatalogueCode,
                GroupCode = drawing.GroupCode,
                SubGroupCode = drawing.SubGroupCode,
                SubSubGroupCode = drawing.SubSubGroupCode,
                DrawingNumber = drawingNumber
            };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);
            model.Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb);
            return View(model);
        }

        private TableViewModel PopulateTableViewModelFromDrawing(DrawingKeyViewModel drawing, string language)
        {
            var tableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.Variant, drawing.Revision, language));
            tableData.MakeCode = drawing.MakeCode;
            tableData.SubMakeCode = drawing.SubMakeCode;
            tableData.ModelCode = drawing.ModelCode;
            tableData.Revision = drawing.Revision;
            tableData.Variant = drawing.Variant;
            return tableData;
        }



    }
}
