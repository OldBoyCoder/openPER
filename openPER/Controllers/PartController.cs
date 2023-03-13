using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
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
            ViewData["Language"] = language;
            p.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language);

            p.Language = language;
            if (partNumber == null) return View("Index", null);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }

        public IActionResult SearchPartByModelAndName(string language, string partModelName, string partName)
        {
            ViewData["Language"] = language;
            if (string.IsNullOrEmpty(partModelName) || string.IsNullOrEmpty(partName)) return NotFound();

            var parts = _rep.GetBasicPartSearch(partModelName, partName, language);
            var model = new PartSearchResultsViewModel();
            model.Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language);

            model.Language = language;
            foreach (var p in parts)
            {
                var v = new PartSearchResultViewModel();
                v.Description = p.Description;
                v.FamilyCode = p.FamilyCode;
                v.FamilyDescription = p.FamilyDescription;
                v.PartNumber = p.PartNumber;
                v.UnitOfSale = p.UnitOfSale;
                v.Weight = p.Weight;
                v.CatalogueCode = p.CatalogueCode;
                v.CatalogueDescription = p.CatalogueDescription;
                v.Drawings = p.Drawings;
                model.Results.Add(v);

            }
            return View("SearchResults", model);
        }
    }
}
