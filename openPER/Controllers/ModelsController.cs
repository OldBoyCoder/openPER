using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;
using openPERHelpers;

namespace openPER.Controllers
{
    public class ModelsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public ModelsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("{language}/Models/{MakeCode}/{SubMakeCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var model = new ModelsViewModel
            {
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode, subMakeCode)),
                MakeCode = makeCode,
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode)
            };
            return View(model);

        }
    }
}
