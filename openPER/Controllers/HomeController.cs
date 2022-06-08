using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNetCore.Http;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVersionedRepository _rep;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IVersionedRepository repository, IConfiguration config)
        {
            _logger = logger;
            _rep = repository;
            _config = config;
        }

        public IActionResult Index()
        {
            var model = new SessionOptionsViewModel
            {
                Languages = _rep.GetAllLanguages(18)
            };
            if (HttpContext.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                model.CurrentLanguage = HttpContext.Request.Cookies["PreferredLanguage"];
            }
            var s = _config.GetSection("Releases").Get<ReleaseModel[]>();
            model.Versions = new List<VersionModel>();
            foreach (var release in s)
            {
                var v = new VersionModel
                {
                    Release = release.Release,
                    Description = release.Description
                };
                model.Versions.Add(v);

            }

            model.CurrentVersion = model.Versions[0].Release;
            return View( model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SetLanguage(SessionOptionsViewModel model)
        {
            HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage);
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }

        [HttpPost]
        public IActionResult Index(SessionOptionsViewModel model)
        {
            HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage);
            HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return RedirectToAction("Index", "Makes", new { ReleaseCode =model.CurrentVersion});
        }

    }
}
