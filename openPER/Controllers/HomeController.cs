using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.Interfaces;
using openPER.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace openPER.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IRepository rep;


        public HomeController(ILogger<HomeController> logger, IRepository repository)
        {
            _logger = logger;
            rep = repository;
        }

        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            return View(rep.GetAllMakes());
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
        public IActionResult Index(LanguagesViewModel model)
        {
            HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage);
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return View(rep.GetAllMakes());
        }

    }
}
