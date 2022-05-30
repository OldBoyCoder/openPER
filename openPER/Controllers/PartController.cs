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
        [Route("Part/{PartNumber}")]
        public ActionResult Index(string partNumber)
        {
            var p = new PartViewModel();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            p.PartNumberSearch = partNumber;

            p.Result = rep.GetPartDetails(p.PartNumberSearch, language);
            return View(p);
        }

    }
}
