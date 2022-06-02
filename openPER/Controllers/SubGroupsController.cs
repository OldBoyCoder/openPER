using AutoMapper;
using System.Collections.Generic;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class SubGroupsController : Controller
    {
        IVersionedRepository _rep;
        IMapper _mapper;
        public SubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{ReleaseCode}/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(int releaseCode, string catalogueCode, int groupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new SubGroupsViewModel();
            model.SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(releaseCode,catalogueCode, groupCode, language));
            model.CatalogueCode = catalogueCode;
            model.GroupCode = groupCode;
            return View(model);
        }
    }
}
