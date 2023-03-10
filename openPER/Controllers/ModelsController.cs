using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;

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
        [Route("Models/{language}/{MakeCode}/{SubMakeCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode)
        {
            // Standard prologue
            LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var model = new ModelsViewModel
            {
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode, subMakeCode)),
                MakeCode = makeCode,
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode)
            };
            return View(model);

        }
    }
}
