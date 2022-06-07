using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;

namespace openPER.Controllers
{
    public class ModelsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public ModelsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Models/{ReleaseCode}/{MakeCode}/{SubMakeCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string subMakeCode)
        {
            // Standard prologue
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);


            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new ModelsViewModel
            {
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(releaseCode,makeCode, subMakeCode)),
                ReleaseCode = releaseCode,
                MakeCode = makeCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(releaseCode, makeCode,
                            subMakeCode))
                    }
                }

            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);

        }
    }
}
