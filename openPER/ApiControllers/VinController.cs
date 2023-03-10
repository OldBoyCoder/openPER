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
        private readonly string _pathToVindataCh;
        private readonly string _pathToVindataRt;
        public VinController(IConfiguration config)
        {
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s?.FirstOrDefault(x => x.Release == 84);
            if (release == null) return;
            _pathToVindataCh = release.VinDataCh;
            _pathToVindataRt = release.VinDataRt;
        }

        [Route("api/Vin/FindVehiclesByFullVin/{FullVin}")]
        [HttpGet]
        public ActionResult<IEnumerable<VinResult>> FindVehiclesByFullVin(string fullVin)
        {
            var x = new Release84VinSearch(_pathToVindataCh, _pathToVindataRt);

            if (string.IsNullOrEmpty(fullVin) || fullVin.Length != 17)
                return NotFound();

            var searchResult = x.FindVehicleByModelAndChassis(fullVin.Substring(3, 3), fullVin.Substring(9, 8));
            var c = new List<VinResult> { searchResult };
            return c;
        }
    }
}
