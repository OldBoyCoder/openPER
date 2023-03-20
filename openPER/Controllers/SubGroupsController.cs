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
    public class SubGroupsController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public SubGroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, string vin = "", string mvs = "")
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);

            var mapDetails = _rep.GetMapForCatalogueGroup(makeCode, subMakeCode, modelCode, catalogueCode, groupCode);
            
            var model = new SubGroupsViewModel
            {
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language)),
                MapEntries = _mapper.Map<List<SubGroupImageMapEntryModel>, List<SubGroupImageMapEntryViewModel>>(_rep.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, language)),
                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                ImagePath = mapDetails.ImageName,
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, vin, mvs),

            };

            return View(model);
        }
    }
}
