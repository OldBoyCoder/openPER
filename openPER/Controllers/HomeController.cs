using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using openPERHelpers;

namespace openPER.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _rep;
        private readonly IMapper _mapper;

        public HomeController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return Index("en");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("{language=en}/Home/Index")]
        public IActionResult Index(string language)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language)
            };

            return View(model);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var ehpf = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var e = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Path = ehpf == null ? "" : ehpf.Path
            };
            return View(e);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SetLanguage(SessionOptionsViewModel model)
        {
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        [Route("Home/SetLanguage/{LanguageCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SetLanguage(string languageCode)
        {
            var newCulture = LanguageSupport.ConvertLanguageCodeToCulture(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(SessionOptionsViewModel model)
        {
            var newCulture = LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return RedirectToAction("Index", "Makes");
        }

    }
}
