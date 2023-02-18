using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using VinSearcher;

namespace openPER.Controllers
{
    public class VinSearchController : Controller
    {
        private readonly ILogger<VinSearchController> _logger;
        private readonly IRepository _rep;

        private readonly IConfiguration _config;
        private readonly string _pathToVindataCH;
        private readonly string _pathToVindataRT;

        public VinSearchController(ILogger<VinSearchController> logger, IRepository repository, IConfiguration config)
        {
            _logger = logger;
            _rep = repository;
            _config = config;
            var s = _config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 84);
            if (release != null)
            {
                _pathToVindataCH = release.VinDataCH;
                _pathToVindataRT = release.VinDataRT;
            }

        }

        public IActionResult Index()
        {
            var model = new VinSearchViewModel
            {
                Models = _rep.GetAllVinModels()
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult SearchByChassisAndVin(string selectedModel, string chassisNumber)
        {
            VinSearchViewModel vinSearch = null;

            var x = new Release84VinSearch(_pathToVindataCH, _pathToVindataRT);
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (selectedModel != null && chassisNumber != null)
            {
                var searchResult = x.FindVehicleByModelAndChassis(selectedModel.PadLeft(3, '0'), chassisNumber.PadLeft(8, '0'));
                if (searchResult != null)
                {
                    vinSearch = new VinSearchViewModel();
                    vinSearch.MvsData = _rep.GetMvsDetails(searchResult.Marque, searchResult.Model, searchResult.Version, searchResult.Series, searchResult.Guide, searchResult.ShopEquipment, searchResult.InteriorColour, language);
                    vinSearch.ChassisNumber = searchResult.Chassis;
                    vinSearch.Organization = searchResult.Organization;
                    vinSearch.ProductionDate = searchResult.Date;
                    vinSearch.EngineNumber = searchResult.Motor;
                    vinSearch.Models = _rep.GetAllModels();
                }
            }
            return View("Index", vinSearch);
        }
        [HttpGet]
        public IActionResult SearchByFullVin(string fullVin)
        {
            var x = new Release84VinSearch(_pathToVindataCH, _pathToVindataRT);

            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return View("Index", null);

            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3, 3), fullVin.Substring(9, 8));
            VinSearchViewModel vinSearch = null;
            if (searchResult != null)
            {
                vinSearch = new VinSearchViewModel();
                vinSearch.MvsData = _rep.GetMvsDetails(searchResult.Marque, searchResult.Model, searchResult.Version, searchResult.Series, searchResult.Guide, searchResult.ShopEquipment, searchResult.InteriorColour, language);
                vinSearch.ChassisNumber = searchResult.Chassis;
                vinSearch.Organization = searchResult.Organization;
                vinSearch.ProductionDate = searchResult.Date;
                vinSearch.EngineNumber = searchResult.Motor;
                vinSearch.InteriorColourCode = searchResult.InteriorColour;
                vinSearch.ExteriorColourCode = searchResult.ExteriorColour;
                if (vinSearch.MvsData.Count > 0)
                {
                    var catCode = vinSearch.MvsData[0].CatalogueCode;
                    if (vinSearch.InteriorColourCode != null)
                        vinSearch.InteriorColourDesc = _rep.GetInteriorColourDescription(catCode, vinSearch.InteriorColourCode, language);
                    if (vinSearch.ExteriorColourCode != null)
                        vinSearch.ExteriorColourDesc = _rep.GetExteriorColourDescription(catCode, vinSearch.ExteriorColourCode, language);
                    // Look at the caratt field if we have it
                    vinSearch.Options = new List<VinSearchOptionsViewModel>();
                    if (!string.IsNullOrEmpty(searchResult.Caratt))
                    {
                        var carattCode = searchResult.Caratt;
                        var parts = searchResult.Caratt.Split(new char[] { '+' });
                        foreach (var part in parts)
                        {
                            var o = new VinSearchOptionsViewModel();
                            if (part.Contains('|'))
                            {
                                var split = part.Split(new char[] { '|' });
                                o.Sequence = 0;
                                o.Code = split[0];
                                o.Value = split[1];
                                switch (o.Code)
                                {
                                    case "COLEST":
                                        o.CodeDescription = "Exterior colour";
                                        o.ValueDescription = _rep.GetExteriorColourDescription(catCode, o.Value, language);
                                        break;
                                    case "COLINT":
                                        o.CodeDescription = "Interior colour";
                                        o.ValueDescription = _rep.GetInteriorColourDescription(catCode, o.Value, language);
                                        break;
                                    default:
                                        o.CodeDescription = _rep.GetOptionCodeDescription(catCode, o.Code, language);
                                        o.Value = split[1];
                                        o.ValueDescription = _rep.GetOptionValueDescription(catCode, o.Code, o.Value, language);
                                        break;
                                }
                            }
                            else
                            {
                                o.Code = part;
                                o.Sequence = 1;
                                o.CodeDescription = _rep.GetOptionValueDescription(catCode, o.Code, language);
                                o.Value = "ZZZZZZZ";
                                o.ValueDescription = "Yes";
                            }
                            vinSearch.Options.Add(o);
                        }
                    }


                }
                vinSearch.Models = _rep.GetAllModels();
            }
            return View("Index", vinSearch);
        }
    }
}
