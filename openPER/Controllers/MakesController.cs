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
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public MakesController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }
        [Route("Makes")]
        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var breadcrumb = new BreadcrumbModel ();
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);

            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                    }
                }

            };
            return View(model);
        }
    }
}
