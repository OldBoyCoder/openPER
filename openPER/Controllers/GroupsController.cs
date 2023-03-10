using AutoMapper;
using System.Collections.Generic;
using openPER.ViewModels;
using Microsoft.AspNetCore.Mvc;
using openPERModels;
using openPERRepositories.Interfaces;
using openPER.Helpers;

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
        public IActionResult Index(string language, string makeCode,string subMakeCode, string modelCode, string catalogueCode, string vin = "", string mvs = "")
        {
            // Standard prologue
            LanguageSupport.SetCultureBasedOnRoute(language);
            ViewData["Language"] = language;

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
                Navigation = NavigationHelper.PopulateNavigationModel(_mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, vin, mvs),
                ModelVariants = _mapper.Map<List<CatalogueVariantsModel>, List<CatalogueVariantsViewModel>>( _rep.GetCatalogueVariants(catalogueCode))
            };

            return View(model);
        }
    }
}
