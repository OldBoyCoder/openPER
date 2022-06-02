using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class MakesController : Controller
    {
        IVersionedRepository _rep;
        IMapper _mapper;
        public MakesController(IVersionedRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }
        [Route("Makes/{ReleaseCode}")]
        public IActionResult Index(int releaseCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode))
            };
            model.ReleaseCode = releaseCode;
            return View(model);
        }
    }
}
