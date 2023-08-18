using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class VinSearchController : Controller
    {
        private readonly IRepository _rep;
        private readonly IMapper _mapper;

        public VinSearchController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult SearchByFullVin(string language, string fullVin)
        {

            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var results = new VinSearchResultsViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language)
            };

            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
            {
                results.Results = new List<VinSearchResultViewModel>();
                return View("Index", results);
            }
            var searchResults = _rep.FindMatchesForVin(language, fullVin);
            results.Results = _mapper.Map<List<VinSearchResultModel>, List<VinSearchResultViewModel>>(searchResults);
            foreach (var result in results.Results)
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
