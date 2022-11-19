using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class SubGroupsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public SubGroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode };

            var model = new SubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language)),
                MapEntries = _mapper.Map<List<SubGroupImageMapEntryModel>, List<SubGroupImageMapEntryViewModel>>(_rep.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, language)),

                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                    subMakeCode)),
                        AllCatalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(
                    _rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language)),
                        AllGroups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(
                    _rep.GetGroupsForCatalogue(catalogueCode, language)),
                        AllSubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(
                    _rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language))
                    }
                }


            };

            return View(model);
        }
    }
}
