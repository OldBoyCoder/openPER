using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class CataloguesController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;

        public CataloguesController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Catalogues/{MakeCode}/{SubMakeCode}/{ModelCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string makeCode, string subMakeCode, string modelCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);

            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(_rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language)),
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                            subMakeCode)),
                        AllCatalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(
                            _rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language))
                    }
                },
                ImagePath = _rep.GetImageNameForModel(makeCode, subMakeCode, modelCode)

            };
            model.Models =
                _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                    subMakeCode));


            return View(model);
        }
    }
}
