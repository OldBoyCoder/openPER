using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;

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
        [Route("Catalogues/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode)
        {
            // Standard prologue
            LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(_rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language)),
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode),
                ImagePath = _rep.GetImageNameForModel(makeCode, subMakeCode, modelCode),
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                    subMakeCode))
            };


            return View(model);
        }
    }
}
