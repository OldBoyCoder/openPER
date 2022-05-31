using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.Interfaces;
using openPER.ViewModels;
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
        [HttpGet]
        public IActionResult SearchByChassisAndVin(string selectedModel, string chassisNumber)
        {
            VinSearchViewModel vinSearch = null;
            var x = new VinSearch();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (selectedModel != null && chassisNumber != null)
            {
                var searchResult = x.FindVehicleByModelAndChassis(selectedModel.PadLeft(3, '0'), chassisNumber.PadLeft(8, '0'));
                if (searchResult != null)
                {
                    vinSearch = new VinSearchViewModel();

                    var mvsCode = searchResult.MVS.Substring(0, 3);
                    var mvsVersion = searchResult.MVS.Substring(3, 3);
                    var mvsSeries = searchResult.MVS.Substring(6, 1);
                    vinSearch.MvsData = rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
                    vinSearch.ChassisNumber = searchResult.Chassis;
                    vinSearch.Organization = searchResult.Organization;
                    vinSearch.ProductionDate = searchResult.Date;
                    vinSearch.EngineNumber = searchResult.Motor;
                }
            }
            return View("Index", vinSearch);
        }
        [HttpGet]
        public IActionResult SearchByFullVin(string fullVin)
        {
            var x = new VinSearch();

            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return View("Index", null);
            
            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3,3),fullVin.Substring(9, 8));
            VinSearchViewModel vinSearch = null;
            if (searchResult != null)
            {
                vinSearch = new VinSearchViewModel();
                var mvsCode = searchResult.MVS.Substring(0, 3);
                var mvsVersion = searchResult.MVS.Substring(3, 3);
                var mvsSeries = searchResult.MVS.Substring(6, 1);
                vinSearch.MvsData = rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
                vinSearch.ChassisNumber = searchResult.Chassis;
                vinSearch.Organization = searchResult.Organization;
                vinSearch.ProductionDate = searchResult.Date;
                vinSearch.EngineNumber = searchResult.Motor;


            }
            vinSearch.Models = rep.GetAllModels();
            return View("Index", vinSearch);
        }
    }
}
