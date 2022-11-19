using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class SubSubGroupsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public SubSubGroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubSubGroups/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        public IActionResult Index(string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, 
                CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode};
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);

            var model = new SubSubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language)),
                SubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(_rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode,subGroupCode, language)),
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,  
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
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
                            _rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language)),
                        AllSubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(
                            _rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode,subGroupCode, language))
                    }
                }
            };

            return View(model);
        }

    }
}
