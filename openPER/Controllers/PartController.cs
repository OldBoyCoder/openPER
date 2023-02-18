using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

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
            var p = new PartSearchViewModel();
            return View(p);
        }

        [HttpGet]
        public ActionResult SearchResults(string partNumber)
        {
            var p = new PartSearchViewModel();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }
    }
}
