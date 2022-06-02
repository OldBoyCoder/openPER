using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        readonly IRepository _rep;
        public PartController(IRepository rep)
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
            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }
    }
}
