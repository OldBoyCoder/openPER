using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class MakesController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public MakesController(IVersionedRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }
        [Route("Makes/{ReleaseCode}")]
        public IActionResult Index(int releaseCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var breadcrumb = new BreadcrumbModel ();
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                ReleaseCode = releaseCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                    }
                }

            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;
            return View(model);
        }
    }
}
