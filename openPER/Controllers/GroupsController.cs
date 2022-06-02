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
        IRepository _rep;
        IMapper _mapper;
        public GroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Groups/{CatalogueCode}")]
        public IActionResult Index(string catalogueCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            var model = new GroupsViewModel
            {
                Groups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(_rep.GetGroupsForCatalogue(catalogueCode, language))
            };

            model.CatalogueCode = catalogueCode; 
            return View(model);
        }
    }
}
