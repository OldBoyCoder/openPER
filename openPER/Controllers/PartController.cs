using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        readonly IRepository _rep;
        private readonly IMapper _mapper;
        public PartController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var p = new PartSearchViewModel();
            return View(p);
        }

        [HttpGet]

        public ActionResult SearchPartByPartNumber(string language, string partNumber)
        {
            var p = new PartSearchViewModel();
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            p.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language);

            p.Language = language;
            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }

        public IActionResult SearchPartByModelAndName(string language, string partModelName, string partName)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            if (string.IsNullOrEmpty(partModelName) || string.IsNullOrEmpty(partName)) return NotFound();

            var parts = _rep.GetBasicPartSearch(partModelName, partName, language);
            var model = new PartSearchResultsViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language),
                Language = language
            };

            foreach (var p in parts)
            {
                var v = new PartSearchResultViewModel
                {
                    Description = p.Description,
                    FamilyCode = p.FamilyCode,
                    FamilyDescription = p.FamilyDescription,
                    PartNumber = p.PartNumber,
                    UnitOfSale = p.UnitOfSale,
                    Weight = p.Weight,
                    CatalogueCode = p.CatalogueCode,
                    CatalogueDescription = p.CatalogueDescription,
                    Drawings = p.Drawings
                };
                model.Results.Add(v);

            }
            return View("SearchResults", model);
        }
    }
}
