using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using VinSearcher;

namespace openPER.ApiControllers
{
    [ApiController]
    public class VinController : ControllerBase
    {

        [Route("api/Vin/FindVehiclesByFullVin/{FullVin}")]
        public ActionResult<IEnumerable<VinResult>> FindVehiclesByFullVin(string fullVin)
        {
            var x = new VinSearch();

            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return NotFound();

            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3, 3), fullVin.Substring(9, 8));
            var c = new List<VinResult> { searchResult };
            return c;
        }
    }
}
