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
        IRepository rep;
        IMapper mapper;
        public MakesController(IRepository _repository, IMapper _mapper)
        {
            rep = _repository;
            mapper = _mapper;
        }
        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new MakesViewModel
            {
                Makes = mapper.Map<List<MakeModel>, List<MakeViewModel>>(rep.GetAllMakes())
            };
            return View(model);
        }
    }
}
