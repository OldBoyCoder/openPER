using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class DrawingsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public DrawingsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("Detail/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}")]
        public IActionResult Detail(int releaseCode, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel();
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode,
                GroupCode = groupCode, SubGroupCode = subGroupCode,SubSubGroupCode =subSubGroupCode, DrawingNumber = drawingNumber
            };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);
            model.Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb);
            model.Breadcrumb.ReleaseCode = releaseCode;

            model.ReleaseCode = releaseCode;
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubSubGroup(releaseCode, makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            model.Drawings.ForEach(x => x.SubMakeCode = subMakeCode);
            // Now we get the rest of the details for the drawing we're interested in
            var drawing = model.Drawings[drawingNumber - 1];
            // Get the table for this drawing
            model.TableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.ReleaseCode, drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.DrawingNumber, language));
            model.TableData.MakeCode = makeCode;
            model.TableData.SubMakeCode = subMakeCode;
            model.TableData.ModelCode = modelCode;
            model.TableData.CurrentDrawing = drawingNumber;

            return View(model);
        }
        [Route("Detail/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{DrawingNumber}")]
        public IActionResult Detail(int releaseCode, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel
            {
                ReleaseCode = releaseCode
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCatalogue(releaseCode, makeCode, modelCode,
                catalogueCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            // Now we get the rest of the details for the drawing we're interested in
            var drawing = model.Drawings[drawingNumber - 1];
            // Get the table for this drawing
            model.TableData = _mapper.Map<TableModel, TableViewModel>(
                _rep.GetTable(drawing.ReleaseCode, drawing.CatalogueCode, drawing.GroupCode, drawing.SubGroupCode,
                    drawing.SubSubGroupCode, drawing.DrawingNumber, language));
            model.TableData.MakeCode = makeCode;
            model.TableData.SubMakeCode = subMakeCode;
            model.TableData.ModelCode = modelCode;

            return View(model);
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel
            {
                ReleaseCode = releaseCode
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubSubGroup(releaseCode, makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            model.RequestType = DrawingsRequestType.SubSubGroup;
            return View(model);
        }
        // A very vague route to a large set of drawings!
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel
            {
                ReleaseCode = releaseCode
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCatalogue(releaseCode, makeCode, modelCode,
                catalogueCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            model.RequestType = DrawingsRequestType.Catalogue;
            return View(model);
        }
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel
            {
                ReleaseCode = releaseCode
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForGroup(releaseCode, makeCode, modelCode, catalogueCode, groupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            model.RequestType = DrawingsRequestType.Group;
            return View(model);
        }
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel
            {
                ReleaseCode = releaseCode
            };
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubGroup(releaseCode, makeCode, modelCode, catalogueCode, groupCode, subGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            model.RequestType = DrawingsRequestType.SubGroup;
            return View(model);
        }

    }
}
