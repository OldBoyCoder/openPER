using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class ModelsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public ModelsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Models/{language}/{MakeCode}/{SubMakeCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language,string makeCode, string subMakeCode)
        {
            // Standard prologue
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);


            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);

            var model = new ModelsViewModel
            {
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode, subMakeCode)),
                MakeCode = makeCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                            subMakeCode))
                    },
                    Language = language
                }

            };
            return View(model);

        }
    }
}
