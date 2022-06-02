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
        IVersionedRepository rep;
        IMapper _mapper;

        public CataloguesController(IVersionedRepository _rep, IMapper mapper)
        {
            rep = _rep;
            _mapper = mapper;
        }
        [Route("Catalogues/{ReleaseCode}/{MakeCode}/{ModelCode}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(rep.GetAllCatalogues(releaseCode, makeCode, modelCode, language))
            };
            model.ReleaseCode = releaseCode;

            return View(model);
        }
    }
}
