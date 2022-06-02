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
        IRepository _rep;
        IMapper _mapper;
        public MakesController(IRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes())
            };
            return View(model);
        }
    }
}
