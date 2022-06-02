using AutoMapper;
using System.Collections.Generic;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class SubSubGroupsController : Controller
    {
        IVersionedRepository _rep;
        IMapper _mapper;
        public SubSubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubSubGroups/{ReleaseCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        public IActionResult Index(int releaseCode, string catalogueCode, int groupCode, int subGroupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new SubSubGroupsViewModel();
            model.SubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(_rep.GetSubSubGroupsForCatalogueGroupSubGroup(releaseCode, catalogueCode, groupCode,subGroupCode, language));
            model.CatalogueCode = catalogueCode;
            model.GroupCode = groupCode;
            model.SubGroupCode = subGroupCode;
            return View(model);
        }

    }
}
