using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

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
            var p = new PartSearchViewModel();
            return View(p);
        }

        [HttpGet]
        public ActionResult SearchResults(string partNumber)
        {
            var p = new PartSearchViewModel();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);

            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(releaseCode, p.PartNumberSearch, language);
            p.ReleaseCode = releaseCode;
            return View("Index", p);
        }
    }
}
