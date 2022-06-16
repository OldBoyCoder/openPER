using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using openPERModels;
using VinSearcher;

namespace openPER.ApiControllers
{
    [ApiController]
    public class VinController : ControllerBase
    {
        private IConfiguration _config;
        private string _pathToVindata;
        public VinController(IConfiguration config)
        {
                _config = config;
                var s = _config.GetSection("Releases").Get<ReleaseModel[]>();
                var release = s.FirstOrDefault(x => x.Release == 18);
                if (release != null)
                {
                    _pathToVindata = release.VinData;
                }

        }

        [Route("api/Vin/FindVehiclesByFullVin/{FullVin}")]
        public ActionResult<IEnumerable<VinResult>> FindVehiclesByFullVin(string fullVin)
        {
            var x = new Release18VinSearch(_pathToVindata);

            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return NotFound();

            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3, 3), fullVin.Substring(9, 8));
            var c = new List<VinResult> { searchResult };
            return c;
        }
    }
}
