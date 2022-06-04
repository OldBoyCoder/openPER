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
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public SubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(int releaseCode,string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new SubGroupsViewModel
            {
                Breadcrumb = new BreadcrumbViewModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode},
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(releaseCode,catalogueCode, groupCode, language)),
                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                ReleaseCode = releaseCode

            };
            return View(model);
        }
    }
}
