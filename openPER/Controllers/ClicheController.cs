using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
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
            "Detail/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{ClichePartNumber}/{ClicheDrawingNumber}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber,
            string clichePartNumber, int clicheDrawingNumber, string VIN = "", string MVS = "")
        {
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var model = new ClicheViewModel();

            model.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode,subSubGroupCode, drawingNumber, clichePartNumber, clicheDrawingNumber, VIN, MVS);

            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode,
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
