using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers.API
{
    [Route("api/Part")]
    [ApiController]
    public class PartApiController : ControllerBase
    {
        readonly IRepository _rep;

        public PartApiController(IRepository rep)
        {
            _rep = rep;
        }
        [HttpGet("{language}/{partCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartSearchViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPart(string language, string partCode)
        {
            var p = new PartSearchViewModel();

            if (partCode == null) return NotFound();
            p.PartNumberSearch = partCode;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return p.Result == null ? NotFound() : Ok(p);
        }
        [HttpGet("{language}/{ModelName}/{PartDescription}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PartSearchViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPart(string language, string modelName, string partDescription)
        {
            if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(partDescription)) return NotFound();

            var p = _rep.GetPartSearch(modelName, partDescription, language);
            return p.Count == 0 ? NotFound() : Ok(p);
        }
    }
}
