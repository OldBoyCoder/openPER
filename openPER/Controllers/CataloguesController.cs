using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using openPER.Interfaces;
using openPER.Models;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class CataloguesController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;

        public CataloguesController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Catalogues/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string subMakeCode, string modelCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new CataloguesViewModel
            {
                Breadcrumb = new BreadcrumbViewModel{ MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode },
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(_rep.GetAllCatalogues(releaseCode, makeCode,subMakeCode, modelCode, language)),
                ReleaseCode = releaseCode
            };

            return View(model);
        }
    }
}
