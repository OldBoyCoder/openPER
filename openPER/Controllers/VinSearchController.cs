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
        public IActionResult SearchByChassisAndVin(SearchViewModel model)
        {
            var x = new VinSearch();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            model.VinSearch.SelectedModel = model.VinSearch.SelectedModel.PadLeft(3, '0');
            var searchResult = x.FindVehicleByModelAndChassis(model.VinSearch.SelectedModel, model.VinSearch.ChassisNumber);
            if (searchResult != null)
            {
                var mvsCode = searchResult.MVS.Substring(0, 3);
                var mvsVersion = searchResult.MVS.Substring(3, 3);
                var mvsSeries = searchResult.MVS.Substring(6, 1);
                model.VinSearch.MvsData = rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
                model.VinSearch.ChassisNumber = searchResult.Chassis;
                model.VinSearch.Organization = searchResult.Organization;
                model.VinSearch.ProductionDate = searchResult.Date;
                model.VinSearch.EngineNumber = searchResult.Motor;


            }
            model.VinSearch.Models = rep.GetAllModels();
            return View("Index", model.VinSearch);
        }
        [HttpPost]
        public IActionResult SearchByFullVin(SearchViewModel model)
        {
            var x = new VinSearch();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(model.FullVin) || model.FullVin.Length != 17)
                return View();
            
            var searchResult = x.FindVehicleByModelAndChassis(model.FullVin.Substring(3,3), model.FullVin.Substring(9, 8));
            if (searchResult != null)
            {
                var mvsCode = searchResult.MVS.Substring(0, 3);
                var mvsVersion = searchResult.MVS.Substring(3, 3);
                var mvsSeries = searchResult.MVS.Substring(6, 1);
                model.VinSearch.MvsData = rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
                model.VinSearch.ChassisNumber = searchResult.Chassis;
                model.VinSearch.Organization = searchResult.Organization;
                model.VinSearch.ProductionDate = searchResult.Date;
                model.VinSearch.EngineNumber = searchResult.Motor;


            }
            model.VinSearch.Models = rep.GetAllModels();
            return View("Index", model.VinSearch);
        }
    }
}
