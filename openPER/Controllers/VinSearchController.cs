using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.Interfaces;
using openPER.Models;
using VinSearcher;

namespace openPER.Controllers
{
    public class VinSearchController : Controller
    {
        private readonly ILogger<VinSearchController> _logger;
        private IRepository rep;


        public VinSearchController(ILogger<VinSearchController> logger, IRepository repository)
        {
            _logger = logger;
            rep = repository;
        }

        public IActionResult Index()
        {
            var model = new VinSearchViewModel();
            model.Models = rep.GetAllModels();
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(VinSearchViewModel model)
        {
            var x = new VinSearch();
            model.SelectedModel = model.SelectedModel.PadLeft(3, '0');
            var searchResult = x.FindVehicleByModelAndChassis(model.SelectedModel, model.ChassisNumber);
            if (searchResult != null)
            {
                var mvsCode = searchResult.MVS.Substring(0, 3);
                var mvsVersion = searchResult.MVS.Substring(3, 3);
                var mvsSeries = searchResult.MVS.Substring(6, 1);
                model.Result = searchResult;
                model.MvsData = rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries);

            }
            model.Models = rep.GetAllModels();

            return View(model);
        }
    }
}
