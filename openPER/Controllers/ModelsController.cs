using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class ModelsController : Controller
    {
        IRepository rep;
        public ModelsController(IRepository _rep)
        {
            rep = _rep;
        }
        [Route("Models/{MakeCode}")]
        public IActionResult Index(string makeCode)
        {
            var model = new ModelsViewModel();
            model.Models = rep.GetAllModelsForMake(makeCode);
            return View(model);

        }
    }
}
