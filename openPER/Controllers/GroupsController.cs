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
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public GroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Groups/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode,string subMakeCode, string modelCode, string catalogueCode)
        {
            // Standard prologue
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode
            };
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, language);
            var mapAndImage = _rep.GetMapAndImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode);
            var model = new GroupsViewModel
            {
                Groups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(_rep.GetGroupsForCatalogue(catalogueCode, language)),
                MapEntries = _mapper.Map<List<GroupImageMapEntryModel>, List<GroupImageMapEntryViewModel>>(_rep.GetGroupMapEntriesForCatalogue(catalogueCode, language)),
                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                ImagePath = mapAndImage.ImageName,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes()),
                        AllModels = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode,
                            subMakeCode)),
                        AllCatalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(
                            _rep.GetAllCatalogues( makeCode, subMakeCode, modelCode, language)),
                        AllGroups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(
                            _rep.GetGroupsForCatalogue( catalogueCode, language))
                    },
                    Language = language
                }

            };

            return View(model);
        }
    }
}
