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
        IRepository _rep;
        IMapper _mapper;
        public ModelsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Models/{MakeCode}")]
        public IActionResult Index(string makeCode)
        {
            var model = new ModelsViewModel
            {
                Models = _mapper.Map<List<ModelModel>, List<ModelViewModel>>(_rep.GetAllModelsForMake(makeCode))
            };
            return View(model);

        }
    }
}
