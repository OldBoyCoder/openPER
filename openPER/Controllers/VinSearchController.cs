using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public VinSearchController(ILogger<VinSearchController> logger, IRepository repository, IMapper mapper)
        {
            _logger = logger;
            _rep = repository;
            _mapper = mapper;
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
        public IActionResult SearchByFullVin(string language, string fullVin)
        {

            ViewData["Language"] = language;

            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return View("Error", null);
            var searchResults = _rep.FindMatchesForVin(language, fullVin);
            var results = _mapper.Map<List<VinSearchResultModel>, List<VinSearchResultViewModel>>(searchResults);
            foreach (var result in results)
            {
                result.Models = _mapper.Map<List<MvsDataModel>, List<MvsDataViewModel>>(_rep.GetMvsDetails(result.Mvs));
                var vehicleOptions = _rep.GetVehiclePattern(fullVin);

                if (result.Models.Count > 0)
                    result.InteriorColourDescription = _rep.GetInteriorColourDescription(result.Models[0].CatalogueCode, result.InteriorColourCode, language);
                foreach (var model in result.Models)
                {
                    model.Language = language;
                    model.DataSource = "Model";
                    var potentialOptions = _rep.GetMvsDetailsForCatalogue(model.CatalogueCode, language);
                    if (vehicleOptions != "")
                    {
                        model.Pattern = vehicleOptions;
                        model.DataSource = "Vehicle";
                    }


                        var ourOptions = model.Pattern.Split(new[] { '+' });
                    model.Options = new List<System.Tuple<string, string, string, string>>();

                    foreach (var ourOption in ourOptions)
                    {
                        var key = ourOption;
                        var absent = false;
                        if (key.StartsWith("!"))
                        {
                            key = key.Substring(1);
                            absent = true;
                        }
                        var opt = potentialOptions.FirstOrDefault(x => x.TypeCodePair == key);
                        string sortKey;
                        if (opt != null)
                        {
                            if (string.IsNullOrEmpty(opt.TypeDescription))
                                sortKey = "ZZZ" + opt.CodeDescription;
                            else
                                sortKey = "AAA" + opt.TypeDescription;
                            model.Options.Add(new System.Tuple<string, string, string, string>(sortKey, opt.CodeDescription, string.IsNullOrEmpty(opt.TypeDescription) ? absent ? "No" : "Yes" : opt.TypeDescription, opt.TypeCodePair));
                        }
                    }

                }
            }
            return View("Debug", results);
            //VinSearchViewModel vinSearch = null;
            //if (searchResult != null)
            //{
            //    vinSearch = new VinSearchViewModel();
            //    vinSearch.MvsDataModel = _rep.GetMvsDetails(searchResult.Marque, searchResult.Model, searchResult.Version, searchResult.Series, searchResult.Guide, searchResult.ShopEquipment, searchResult.InteriorColour, language);
            //    vinSearch.ChassisNumber = searchResult.Chassis;
            //    vinSearch.Organization = searchResult.Organization;
            //    vinSearch.ProductionDate = searchResult.Date;
            //    vinSearch.EngineNumber = searchResult.Motor;
            //    vinSearch.InteriorColourCode = searchResult.InteriorColour;
            //    vinSearch.ExteriorColourCode = searchResult.ExteriorColour;
            //    if (vinSearch.MvsDataModel.Count > 0)
            //    {
            //        var catCode = vinSearch.MvsDataModel[0].CatalogueCode;
            //        if (vinSearch.InteriorColourCode != null)
            //            vinSearch.InteriorColourDesc = _rep.GetInteriorColourDescription(catCode, vinSearch.InteriorColourCode, language);
            //        if (vinSearch.ExteriorColourCode != null)
            //            vinSearch.ExteriorColourDesc = _rep.GetExteriorColourDescription(catCode, vinSearch.ExteriorColourCode, language);
            //        // Look at the caratt field if we have it
            //        vinSearch.Options = new List<VinSearchOptionsViewModel>();
            //        if (!string.IsNullOrEmpty(searchResult.Caratt))
            //        {
            //            var carattCode = searchResult.Caratt;
            //            var parts = searchResult.Caratt.Split(new char[] { '+' });
            //            foreach (var part in parts)
            //            {
            //                var o = new VinSearchOptionsViewModel();
            //                if (part.Contains('|'))
            //                {
            //                    var split = part.Split(new char[] { '|' });
            //                    o.Sequence = 0;
            //                    o.Code = split[0];
            //                    o.Value = split[1];
            //                    switch (o.Code)
            //                    {
            //                        case "COLEST":
            //                            o.CodeDescription = "Exterior colour";
            //                            o.ValueDescription = _rep.GetExteriorColourDescription(catCode, o.Value, language);
            //                            break;
            //                        case "COLINT":
            //                            o.CodeDescription = "Interior colour";
            //                            o.ValueDescription = _rep.GetInteriorColourDescription(catCode, o.Value, language);
            //                            break;
            //                        default:
            //                            o.CodeDescription = _rep.GetOptionCodeDescription(catCode, o.Code, language);
            //                            o.Value = split[1];
            //                            o.ValueDescription = _rep.GetOptionValueDescription(catCode, o.Code, o.Value, language);
            //                            break;
            //                    }
            //                }
            //                else
            //                {
            //                    o.Code = part;
            //                    o.Sequence = 1;
            //                    o.CodeDescription = _rep.GetOptionValueDescription(catCode, o.Code, language);
            //                    o.Value = "ZZZZZZZ";
            //                    o.ValueDescription = "Yes";
            //                }
            //                vinSearch.Options.Add(o);
            //            }
            //        }


            //    }
            //    vinSearch.Models = _rep.GetAllModels();
            //}
            //return View("Index", vinSearch);
        }
    }
}
