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
        IVersionedRepository _rep;
        IMapper _mapper;
        public GroupsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [Route("Groups/{ReleaseCode}/{CatalogueCode}")]
        public IActionResult Index(int releaseCode, string catalogueCode)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new GroupsViewModel
            {
                Groups = _mapper.Map<List<GroupModel>, List<GroupViewModel>>(_rep.GetGroupsForCatalogue(releaseCode, catalogueCode, language))
            };
            model.ReleaseCode = releaseCode;
            model.CatalogueCode = catalogueCode; 
            return View(model);
        }
    }
}
