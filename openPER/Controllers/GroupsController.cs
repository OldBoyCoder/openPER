using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class GroupsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public GroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Groups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}")]
        public IActionResult Index(int releaseCode,string makeCode,string subMakeCode, string modelCode, string catalogueCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode
            };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new GroupsViewModel
            {
                Groups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(_rep.GetGroupsForCatalogue(releaseCode, catalogueCode, language)),
                MapEntries = _mapper.Map<List<GroupImageMapEntryModel>, List<GroupImageMapEntryViewModel>>(_rep.GetGroupMapEntriesForCatalogue(releaseCode, catalogueCode, language)),
                ReleaseCode = releaseCode,
                MakeCode = makeCode,
                ModelCode = modelCode,  
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
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
                            _rep.GetGroupsForCatalogue(releaseCode, catalogueCode, language))
                    }
                }

            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }
    }
}
