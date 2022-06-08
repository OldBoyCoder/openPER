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
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public SubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new SubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(releaseCode, catalogueCode, groupCode, language)),
                MapEntries = _mapper.Map<List<SubGroupImageMapEntryModel>, List<SubGroupImageMapEntryViewModel>>(_rep.GetSubGroupMapEntriesForCatalogueGroup(releaseCode, catalogueCode, groupCode)),

                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
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
                    _rep.GetSubGroupsForCatalogueGroup(releaseCode, catalogueCode, groupCode, language))
                    }
                }


            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }
    }
}
