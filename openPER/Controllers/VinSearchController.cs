using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.Helpers;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

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
                return View("Index", new List<VinSearchResultViewModel>());
            var searchResults = _rep.FindMatchesForVin(language, fullVin);
            var results = _mapper.Map<List<VinSearchResultModel>, List<VinSearchResultViewModel>>(searchResults);
            foreach (var result in results)
            {
                result.Models = _mapper.Map<List<MvsDataModel>, List<MvsDataViewModel>>(_rep.GetMvsDetails(result.Mvs));

                if (result.Models.Count > 0)
                    result.InteriorColourDescription = _rep.GetInteriorColourDescription(result.Models[0].CatalogueCode, result.InteriorColourCode, language);
                foreach (var model in result.Models)
                {
                    model.Language = language;
                    var f = NavigationHelper.PopulateFilterModel(_mapper, _rep, language, model.CatalogueCode, model.Sincom, fullVin);
                    model.FilterOptions = f;
                }
            }
            return View("Index", results);
        }
    }
}
