using AutoMapper;
using System.Collections.Generic;
using openPER.Interfaces;
using openPER.ViewModels;
using openPER.Models;
using Microsoft.AspNetCore.Mvc;

namespace openPER.Controllers
{
    public class SubGroupsController : Controller
    {
        IRepository _rep;
        IMapper _mapper;
        public SubGroupsController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("SubGroups/{CatalogueCode}/{GroupCode}")]
        public IActionResult Index(string catalogueCode, int groupCode)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var model = new SubGroupsViewModel();
            model.SubGroups = _mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(_rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, language));
            model.CatalogueCode = catalogueCode;
            model.GroupCode = groupCode;
            return View(model);
        }
    }
}
