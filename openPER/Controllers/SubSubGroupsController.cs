using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;
using openPERHelpers;

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
        [Route("{language}/SubSubGroups/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string vin = "", string mvs = "")
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

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
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, vin, mvs)
            };
            if (mvs != "")
            {
                string sinComPattern = _rep.GetSincomPattern(mvs);
                string vehiclePattern= "";
                if (!string.IsNullOrEmpty(vin))
                    vehiclePattern= _rep.GetVehiclePattern(language, vin);
                var vmkCodes = _rep.GetVmkDataForCatalogue(catalogueCode, language);
                var vehicleModificationFilters = new Dictionary<string, string>();
                if (vehiclePattern != "")
                {
                    sinComPattern = vehiclePattern;
                }
                if (!string.IsNullOrEmpty(vin))
                    vehicleModificationFilters = _rep.GetFiltersForVehicle(language, vin, mvs);
                foreach (var d in model.SubSubGroups)
                {
                    var pattern = d.Pattern;
                    var modifications = d.Modifications;
                    d.Visible = PatternMatchHelper.ApplyPatternAndModificationRules(pattern, sinComPattern, vmkCodes,
                        vehiclePattern, modifications, vehicleModificationFilters);
                }
            }
            return View(model);
        }

    }
}
