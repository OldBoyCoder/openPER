using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERModels;
using openPERRepositories.Interfaces;
using System.Collections.Generic;

namespace openPER.Controllers
{
    public class ModificationController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        public ModificationController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }

        [Route("{language}/Modification/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{ModificationNumber}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int modificationNumber, string vin = "", string mvs = "")
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new CatalogueModificationsViewModel();
            model.Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language, makeCode, subMakeCode, modelCode, catalogueCode, vin, mvs);
            model.Details = _mapper.Map<ModificationModel, ModificationViewModel>( _rep.GetCatalogueModificationDetail(catalogueCode, language, modificationNumber));
            model.ChangedDrawings = _mapper.Map<List<ModifiedDrawingModel>, List<ModifiedDrawingViewModel>>(_rep.GetAllDrawingsForModification(language, catalogueCode, modificationNumber));
            return View(model);
        }
    }
}
