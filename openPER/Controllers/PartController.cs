using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        readonly IRepository _rep;
        public PartController(IRepository rep)
        {
            _rep = rep;
        }
        public IActionResult Index()
        {
            var p = new PartSearchViewModel();
            return View(p);
        }

        [HttpGet]

        public ActionResult SearchResults(string language, string partNumber)
        {
            var p = new PartSearchViewModel();
            ViewData["Language"] = language;

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
