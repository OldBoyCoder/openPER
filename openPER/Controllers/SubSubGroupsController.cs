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
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string VIN = "", string MVS = "")
        {
            // Standard prologue
            Helpers.LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

            var model = new SubSubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language)),
                SubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(_rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode, language)),
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, VIN, MVS)
            };
            if (MVS != "")
            {
                string sinComPattern = _rep.GetSincomPattern(MVS);
                string vehiclePattern = _rep.GetVehiclePattern(VIN);
                var vmkCodes = _rep.GetVmkDataForCatalogue(catalogueCode, language);
                if (vehiclePattern != "") sinComPattern = vehiclePattern;
                foreach (var subSubGroup in model.SubSubGroups)
                {
                    var pattern = subSubGroup.Pattern;
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        if (!PatternMatchHelper.EvaluateRule(pattern, sinComPattern, vmkCodes))
                            subSubGroup.Visible = false;
                    }
                    var modifications = subSubGroup.Modifications;
                    var vehicleModificationFilters = _rep.GetFiltersforVehicle(language, VIN, MVS);
                    foreach (var mod in modifications)
                    {
                        foreach (var rule in mod.Activations)
                        {
                            // Does this apply to this vehicle
                            if (PatternMatchHelper.EvaluateRule(rule.ActivationPattern, sinComPattern, vmkCodes))
                            {
                                // Does this vehicle have the data needed
                                if (vehicleModificationFilters.ContainsKey(rule.ActivationCode))
                                {
                                    // Before or after rule?
                                    if (mod.Type == "C")
                                    {
                                        // C means stops at so if data is past this then it is invisible
                                        if (int.Parse(rule.ActivationSpec) <= int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                        {
                                            subSubGroup.Visible = false;
                                        }
                                    }
                                    if (mod.Type == "D")
                                    {
                                        // C means after a date if data is before this then it is invisible
                                        if (int.Parse(rule.ActivationSpec) > int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                        {
                                            subSubGroup.Visible = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return View(model);
        }

    }
}
