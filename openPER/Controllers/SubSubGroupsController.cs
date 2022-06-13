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
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public SubSubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubSubGroups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        public IActionResult Index(int releaseCode, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, 
                CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode};
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new SubSubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(releaseCode, catalogueCode, groupCode, language)),
                SubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(_rep.GetSubSubGroupsForCatalogueGroupSubGroup(releaseCode, catalogueCode, groupCode,subGroupCode, language)),
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,  
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
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
                            _rep.GetAllCatalogues(releaseCode, makeCode, subMakeCode, modelCode, language)),
                        AllGroups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(
                            _rep.GetGroupsForCatalogue(releaseCode, catalogueCode, language)),
                        AllSubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(
                            _rep.GetSubGroupsForCatalogueGroup(releaseCode, catalogueCode, groupCode, language)),
                        AllSubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(
                            _rep.GetSubSubGroupsForCatalogueGroupSubGroup(releaseCode, catalogueCode, groupCode,subGroupCode, language))
                    }
                }
            };

            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }

    }
}
