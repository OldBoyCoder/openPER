using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        IRepository rep;
        public PartController(IRepository _rep)
        {
            rep = _rep;
        }
        public IActionResult Index()
        {
            var p = new PartViewModel();
            return View(p);
        }

        [HttpGet]
        public ActionResult SearchResults(string PartNumber)
        {
            var p = new PartViewModel();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (PartNumber == null) return View("Index", null);
            p.PartNumberSearch = PartNumber;

            p.Result = rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }
    }
}
