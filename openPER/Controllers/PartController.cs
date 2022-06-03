using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        readonly IVersionedRepository _rep;
        public PartController(IVersionedRepository rep)
        {
            _rep = rep;
        }
        public IActionResult Index()
        {
            var p = new PartViewModel();
            return View(p);
        }

        [HttpGet]
        public ActionResult SearchResults(string partNumber)
        {
            var p = new PartViewModel();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var releaseCode = 0;
            if (HttpContext.Request.Cookies.ContainsKey("Release"))
            {
                releaseCode = int.Parse(HttpContext.Request.Cookies["Release"] ?? string.Empty);
            }

            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(releaseCode, p.PartNumberSearch, language);
            return View("Index", p);
        }
    }
}
