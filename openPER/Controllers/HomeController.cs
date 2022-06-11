using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            return View( );
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
        [Route("Home/SetLanguage/{LanguageCode}")]
        public IActionResult SetLanguage(string languageCode)
        {
            HttpContext.Response.Cookies.Append("PreferredLanguage", languageCode);
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        [Route("Home/SetRelease/{ReleaseCode}")]
        public IActionResult SetRelease(int releaseCode)
        {
//            Request.GetTypedHeaders().Referer.ToString()
            HttpContext.Response.Cookies.Append("Release", releaseCode.ToString());
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var pathParts = Request.GetTypedHeaders().Referer.ToString().Split('/').ToArray();
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (int.TryParse(pathParts[i], out int oldRelease))
                {
                    pathParts[i] = releaseCode.ToString();
                    break;
                }
            }

            return Redirect(string.Join('/', pathParts));
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
