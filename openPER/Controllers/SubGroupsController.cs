using openPER.Interfaces;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class SubGroupsController : Controller
    {
        IRepository rep;
        public SubGroupsController(IRepository _rep)
        {
            rep = _rep;
        }
        [Route("SubGroups/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(string catalogueCode, int groupCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new SubGroupsViewModel();
            model.SubGroups = rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language);
            model.CatalogueCode = catalogueCode;
            model.GroupCode = groupCode;
            return View(model);
        }
    }
}
