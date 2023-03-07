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
        }
    }
}
