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
    public class MakesController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public MakesController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }
        [Route("{language}/Makes")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language),
            };
            return View(model);
        }
    }
}
