using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERRepositories.Interfaces;
using System.Text;
using System.Threading.Tasks;

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
            p.Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language);

            p.Language = language;
            if (string.IsNullOrEmpty(partNumber)) return View("Index", p);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }

        public IActionResult SearchPartByModelAndName(string language, string partModelName, string partName)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new PartSearchResultsViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language),
                Language = language
            };
            if (string.IsNullOrEmpty(partModelName) || string.IsNullOrEmpty(partName)) return View("SearchResults", model);

            var parts = _rep.GetBasicPartSearch(partModelName, partName, language);

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

        public FileResult AllParts(string language, string catalogueCode)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var parts = _rep.GetAllPartsForCatalogue(language, catalogueCode);
            var contents = "";
            foreach (var p in parts)
            {
                contents += p.TableCode + "\t"
                    + p.GroupDescription + "\t"
                    + p.SubGroupDescription + "\t"
                    + p.PartCode + "\t"
                    + p.CodeDescription + "\t"
                    + p.ExtraDescription + "\t"
                    + p.Notes + "\t"
                    + p.DrawingNumber + "\t"
                    + p.Rif + "\t"
                    + p.PartPattern + "\t"
                    + p.PartModification + "\t"
                    + p.TablePattern + "\t"
                    + p.TableModification + "\t"
                    + p.Quantity +"\r\n";

            }
            var b = Encoding.UTF8.GetBytes(contents);
            return File(b, "text/tab-separated-values", $"AllParts{catalogueCode}{language}.txt");

        }
    }
}
