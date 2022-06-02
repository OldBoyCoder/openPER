using openPER.Interfaces;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class GroupsController : Controller
    {
        IRepository rep;
        public GroupsController(IRepository _rep)
        {
            rep = _rep;
        }
        [Route("Groups/{CatalogueCode}")]
        public IActionResult Index(string catalogueCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new GroupsViewModel();
            model.Groups = rep.GetGroupsForCatalogue(catalogueCode, language);
            model.CatalogueCode = catalogueCode; 
            return View(model);
        }
    }
}
