using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using System.Collections.Generic;

namespace openPER.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class VinSearchController : ControllerBase
    {
        readonly IRepository _rep;
        private readonly IMapper _mapper;

        public VinSearchController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        [HttpGet("FindByVin/{language}/{fullVin}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VinSearchResultsViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult FindByVin(string language, string fullVin)
        {

            if (string.IsNullOrEmpty(fullVin))
            {
                return NotFound();
            }

            var searchResults = _rep.FindMatchesForVin(language, fullVin);
            var results = _mapper.Map<List<VinSearchResultModel>, List<VinSearchResultViewModel>>(searchResults);
            foreach (var result in results)
            {
                result.Models = _mapper.Map<List<MvsDataModel>, List<MvsDataViewModel>>(_rep.GetMvsDetails(result.Mvs));

                if (result.Models.Count > 0)
                    result.InteriorColourDescription = _rep.GetInteriorColourDescription(result.Models[0].CatalogueCode, result.InteriorColourCode, language);
                foreach (var model in result.Models)
                {
                    model.Language = language;
                    var f = NavigationHelper.PopulateFilterModel(_mapper, _rep, language, model.CatalogueCode, model.Sincom, fullVin);
                    model.FilterOptions = f;
                }
            }

            return results == null ? NotFound() : Ok(results);
        }

    }
}
