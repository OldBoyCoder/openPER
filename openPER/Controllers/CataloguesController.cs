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
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;

        public CataloguesController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Catalogues/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string subMakeCode, string modelCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(_rep.GetAllCatalogues(releaseCode, makeCode, subMakeCode, modelCode, language)),
                ReleaseCode = releaseCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(releaseCode, makeCode,
                            subMakeCode)),
                        AllCatalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(
                            _rep.GetAllCatalogues(releaseCode, makeCode, subMakeCode, modelCode, language))
                    }
                }

            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;
            model.Models =
                _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(releaseCode, makeCode,
                    subMakeCode));


            return View(model);
        }
    }
}
