using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;

namespace openPER.Controllers
{
    public class ModelsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public ModelsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Models/{ReleaseCode}/{MakeCode}/{SubMakeCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string subMakeCode)
        {
            // Standard prologue
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);


            var breadcrumb = new BreadcrumbModel { MakeCode = makeCode, SubMakeCode = subMakeCode };
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new ModelsViewModel
            {
                Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(releaseCode,makeCode, subMakeCode)),
                ReleaseCode = releaseCode,
                MakeCode = makeCode
            };
            return View(model);

        }
    }
}
