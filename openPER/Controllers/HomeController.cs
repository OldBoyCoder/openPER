using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace openPER.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository _rep;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IRepository repository, IConfiguration config,
            IMapper mapper)
        {
            _logger = logger;
            _rep = repository;
            _config = config;
            _mapper = mapper;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("{language=3}")]
        public IActionResult Index(string language)
        {
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language)
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
            var e = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            e.Path = ehpf.Path;
            return View(e);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SetLanguage(SessionOptionsViewModel model)
        {
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        [Route("Home/SetLanguage/{LanguageCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult SetLanguage(string languageCode)
        {
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(SessionOptionsViewModel model)
        {
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return RedirectToAction("Index", "Makes");
        }

    }
}
