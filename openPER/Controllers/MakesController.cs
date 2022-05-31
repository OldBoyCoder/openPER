using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class MakesController : Controller
    {
        IRepository rep;
        public MakesController(IRepository _repository)
        {
            rep = _repository;
        }
        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new MakesViewModel
            {
                Makes = rep.GetAllMakes()
            };
            return View(model);
        }
    }
}
