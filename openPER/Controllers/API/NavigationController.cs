using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers.API
{
    [Route("api/Navigation")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        readonly IRepository _rep;

        public NavigationController(IRepository rep)
        {
            _rep = rep;
        }
        [HttpGet("{language}/WholeHierarchy/")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MakeModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetHierarchy(string language)
        {
            var rc = _rep.GetCatalogueHierarchy(language);
            return Ok(rc);
        }
    }
}
