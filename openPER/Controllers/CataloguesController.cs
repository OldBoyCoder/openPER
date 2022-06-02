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
        IRepository rep;
        IMapper _mapper;

        public CataloguesController(IRepository _rep, IMapper mapper)
        {
            rep = _rep;
            _mapper = mapper;
        }
        [Route("Catalogues/{MakeCode}/{ModelCode}")]
        public IActionResult Index(string makeCode, string modelCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new CataloguesViewModel
            {
                Catalogues = _mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(rep.GetAllCatalogues(makeCode, modelCode, language))
            };

            return View(model);
        }
    }
}
