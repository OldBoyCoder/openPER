﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using openPERRepositories.Interfaces;
using VinSearcher;

namespace openPER.Controllers
{
    public class VinSearchController : Controller
    {
        private readonly ILogger<VinSearchController> _logger;
        private readonly IVersionedRepository _rep;


        public VinSearchController(ILogger<VinSearchController> logger, IVersionedRepository repository)
        {
            _logger = logger;
            _rep = repository;
        }

        public IActionResult Index()
        {
            var releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);
            
            var model = new VinSearchViewModel
            {
                Models = _rep.GetAllModels(releaseCode)
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult SearchByChassisAndVin(string selectedModel, string chassisNumber)
        {
            VinSearchViewModel vinSearch = null;
            var releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);

            var x = new VinSearch();
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (selectedModel != null && chassisNumber != null)
            {
                var searchResult = x.FindVehicleByModelAndChassis(selectedModel.PadLeft(3, '0'), chassisNumber.PadLeft(8, '0'));
                if (searchResult != null)
                {
                    vinSearch = new VinSearchViewModel
                    {
                        ReleaseCode = releaseCode
                    };
                    var mvsCode = searchResult.Mvs.Substring(0, 3);
                    var mvsVersion = searchResult.Mvs.Substring(3, 3);
                    var mvsSeries = searchResult.Mvs.Substring(6, 1);
                    vinSearch.MvsData = _rep.GetMvsDetails(releaseCode, mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
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
            var releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);
            var x = new VinSearch();

            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return View("Index", null);
            
            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3,3),fullVin.Substring(9, 8));
            VinSearchViewModel vinSearch = null;
            if (searchResult != null)
            {
                vinSearch = new VinSearchViewModel();
                var mvsCode = searchResult.Mvs.Substring(0, 3);
                var mvsVersion = searchResult.Mvs.Substring(3, 3);
                var mvsSeries = searchResult.Mvs.Substring(6, 1);
                vinSearch.MvsData = _rep.GetMvsDetails(releaseCode, mvsCode, mvsVersion, mvsSeries, searchResult.InteriorColour, language);
                vinSearch.ChassisNumber = searchResult.Chassis;
                vinSearch.Organization = searchResult.Organization;
                vinSearch.ProductionDate = searchResult.Date;
                vinSearch.EngineNumber = searchResult.Motor;
                vinSearch.Models = _rep.GetAllModels(releaseCode);
                vinSearch.ReleaseCode = releaseCode;
            }
            return View("Index", vinSearch);
        }
    }
}
