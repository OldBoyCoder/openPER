using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;
using openPERHelpers;

namespace openPER.Controllers
{
    public class CataloguesController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;

        public CataloguesController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("{language}/Catalogues/{MakeCode}/{SubMakeCode}/{ModelCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(_rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language)),
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode, modelCode),
                ImagePath = _rep.GetImageNameForModel(makeCode, subMakeCode, modelCode),
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                    subMakeCode))
            };


            return View(model);
        }
    }
}
