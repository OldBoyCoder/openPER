using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class CataloguesController : Controller
    {
        IRepository rep;
        public CataloguesController(IRepository _rep)
        {
            rep = _rep;
        }
        [Route("Catalogues/{MakeCode}/{ModelCode}")]
        public IActionResult Index(string makeCode, string modelCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new CataloguesViewModel();
            model.Catalogues = rep.GetAllCatalogues(makeCode, modelCode, language);
            return View(model);
        }
    }
}
