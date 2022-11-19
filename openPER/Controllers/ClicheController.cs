using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;


namespace openPER.Controllers
{
    public class ClicheController : Controller
    {
        private IRepository _rep;
        private IMapper _mapper;
        public ClicheController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;   
        }
        [Route(
            "Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{ClichePartNumber}/{ClicheDrawingNumber}")]
        public IActionResult Detail(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber,
            decimal clichePartNumber, int clicheDrawingNumber)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new ClicheViewModel();
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
                SubSubGroupCode = subSubGroupCode,
                DrawingNumber = drawingNumber,
                ClichePartNumber = clichePartNumber,
                ClicheDrawingNumber = clicheDrawingNumber
            };
            _rep.PopulateBreadcrumbDescriptions( breadcrumb, language);
            model.Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb);

            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCliche(makeCode,subMakeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber);
            model.ClicheDrawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.ClicheDrawings.ForEach(x => x.SubMakeCode = subMakeCode);
            model.CurrentDrawing = clicheDrawingNumber;
            model.CurrentClicheDrawing = new ClicheDrawingViewModel();
            model.CurrentClicheDrawing.CurrentDrawingNumber = clicheDrawingNumber;
            model.CurrentClicheDrawing.ParentPartNumber = clichePartNumber;
            List<TablePartModel> parts = _rep.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, language);
            model.CurrentClicheDrawing.Parts = _mapper.Map<List<TablePartModel>, List<PartViewModel>>(parts);
            return View(model);
        }
    }
}
