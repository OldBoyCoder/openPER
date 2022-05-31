using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.Interfaces;
using openPER.Models;
using openPER.ViewModels;
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
            var model = new SessionOptionsViewModel();
            model.Languages = rep.GetAllLanguages();
            if (HttpContext.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                model.CurrentLanguage = HttpContext.Request.Cookies["PreferredLanguage"];
            }
            var v = new VersionModel();
            model.Versions = new List<VersionModel>();
            v.Release = 20;
            v.Description = "Release 20";
            model.Versions.Add(v);
            v = new VersionModel();
            v.Release = 84;
            v.Description = "Release 84";
            model.Versions.Add(v);
            model.CurrentVersion = 20;
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
        public IActionResult Index(SessionOptionsViewModel model)
        {
            HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage);
            HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return RedirectToAction("Index", "Makes");
        }

    }
}
