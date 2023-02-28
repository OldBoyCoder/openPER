using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;

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
        [Route("SubSubGroups/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string VIN="", string MVS="")
        {
            // Standard prologue
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, 
                CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode,
                VIN = VIN,
                MVS = MVS
            };
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
                    },
                    Language = language
                }

            };
            if (MVS != "")
            {
                string sinComPattern = _rep.GetSincomPattern(MVS);
                foreach (var subSubGroup in model.SubSubGroups)
                {
                    var pattern = subSubGroup.Pattern;
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        if(!PatternMatchHelper.EvaluateRule(pattern, sinComPattern))
                            subSubGroup.Visible = false;
                    }
                }
            }
            return View(model);
        }

    }
}
