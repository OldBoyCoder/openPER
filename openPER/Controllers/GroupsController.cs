using AutoMapper;
using System.Collections.Generic;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class GroupsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public GroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Groups/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}")]
        public IActionResult Index(int releaseCode,string makeCode,string subMakeCode, string modelCode, string catalogueCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode
            };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new GroupsViewModel
            {
                Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                Groups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(_rep.GetGroupsForCatalogue(releaseCode, catalogueCode, language)),
                ReleaseCode = releaseCode,
                MakeCode = makeCode,
                ModelCode = modelCode,  
                SubMakeCode = subMakeCode,
                CatalogueCode = catalogueCode
            };
            model.Breadcrumb.ReleaseCode = releaseCode;

            return View(model);
        }
    }
}
