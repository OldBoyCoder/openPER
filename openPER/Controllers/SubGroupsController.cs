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
            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode};
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new SubGroupsViewModel
            {
                Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(releaseCode,catalogueCode, groupCode, language)),
                MapEntries = _mapper.Map<List<SubGroupImageMapEntryModel>, List<SubGroupImageMapEntryViewModel>>(_rep.GetSubGroupMapEntriesForCatalogueGroup(releaseCode, catalogueCode, groupCode)),

                MakeCode = makeCode,
                ModelCode = modelCode,
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                ReleaseCode = releaseCode

            };
            model.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }
    }
}
