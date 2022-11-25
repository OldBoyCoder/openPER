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
        [Route("Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}")]
        public IActionResult Detail(string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel();
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode,
                GroupCode = groupCode, SubGroupCode = subGroupCode,SubSubGroupCode =subSubGroupCode, DrawingNumber = drawingNumber
            };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);
            model.Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb);
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.SubMakeCode = subMakeCode);
            // Now we get the rest of the details for the drawing we're interested in
            var drawing = model.Drawings[drawingNumber-1];
            // Get the table for this drawing
            model.TableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.Variant,drawing.Revision, language));
            model.TableData.MakeCode = makeCode;
            model.TableData.SubMakeCode = subMakeCode;
            model.TableData.ModelCode = modelCode;
            model.TableData.Revision = drawing.Revision;
            model.TableData.Variant = drawing.Variant;
            model.TableData.CurrentDrawing = drawingNumber;

            return View(model);
        }
        [Route("Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{DrawingNumber}/{Revision}")]
        public IActionResult Detail(string makeCode,string subMakeCode, string modelCode, string catalogueCode, int drawingNumber, int revision)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel
            {
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCatalogue(makeCode, modelCode,
                catalogueCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            // Now we get the rest of the details for the drawing we're interested in
            var drawing = model.Drawings[drawingNumber - 1];
            // Get the table for this drawing
            model.TableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.Variant,revision, language));
            model.TableData.MakeCode = makeCode;
            model.TableData.SubMakeCode = subMakeCode;
            model.TableData.ModelCode = modelCode;

            return View(model);
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("Drawings/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel
            {
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.RequestType = DrawingsRequestType.SubSubGroup;
            return View(model);
        }
        // A very vague route to a large set of drawings!
        [Route("Drawings/{MakeCode}/{ModelCode}/{CatalogueCode}/{DrawingNumber}")]
        public IActionResult Index(string makeCode, string modelCode, string catalogueCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel
            {
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCatalogue(makeCode, modelCode,
                catalogueCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.RequestType = DrawingsRequestType.Catalogue;
            return View(model);
        }
        [Route("Drawings/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{DrawingNumber}")]
        public IActionResult Index(string makeCode, string modelCode, string catalogueCode, int groupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel
            {
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.RequestType = DrawingsRequestType.Group;
            return View(model);
        }
        [Route("Drawings/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new DrawingsViewModel
            {
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.RequestType = DrawingsRequestType.SubGroup;
            return View(model);
        }

    }
}
