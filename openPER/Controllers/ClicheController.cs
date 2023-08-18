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
    public class ClicheController : Controller
    {
        private readonly IRepository _rep;
        private readonly IMapper _mapper;
        readonly List<SearchEngineViewModel> PartSearchUrl;
        public ClicheController(IRepository rep, IMapper mapper, IConfiguration config)
        {
            _rep = rep;
            _mapper = mapper;
            PartSearchUrl = NavigationHelper.GetSearchEnginesFromConfig(config);
        }
        [Route(
            "{language}/Cliche/Detail/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{Scope}/{ClichePartNumber}/{ClicheDrawingNumber}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Detail(string language, string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope,
            string clichePartNumber, int clicheDrawingNumber, string vin = "", string mvs = "")
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var model = new ClicheViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, clichePartNumber, clicheDrawingNumber, vin, mvs)
            };

            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber);
            model.ClicheDrawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.ClicheDrawings.ForEach(x => x.SubMakeCode = subMakeCode);
            model.CurrentDrawing = clicheDrawingNumber;
            model.CurrentClicheDrawing = new ClicheDrawingViewModel
            {
                CurrentDrawingNumber = clicheDrawingNumber,
                ParentPartNumber = clichePartNumber
            };
            List<TablePartModel> parts = _rep.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, language);
            model.CurrentClicheDrawing.Parts = _mapper.Map<List<TablePartModel>, List<PartViewModel>>(parts);
            model.PartSearchUrl = PartSearchUrl;

            return View(model);
        }
    }
}
