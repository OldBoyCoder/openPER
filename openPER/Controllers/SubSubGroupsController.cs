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
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public SubSubGroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubSubGroups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}")]
        public IActionResult Index(int releaseCode, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, 
                CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode};
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new SubSubGroupsViewModel
            {
                Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                SubSubGroups = _mapper.Map<List<SubSubGroupModel>, List<SubSubGroupViewModel>>(_rep.GetSubSubGroupsForCatalogueGroupSubGroup(releaseCode, catalogueCode, groupCode,subGroupCode, language)),
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,  
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
                ReleaseCode = releaseCode

            };
            model.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }

    }
}
