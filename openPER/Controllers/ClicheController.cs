using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace openPER.Controllers
{
    public class ClicheController : Controller
    {
        [Route(
            "Detail/{ReleaseCode}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{ClichePartNumber}")]
        public IActionResult Detail(int releaseCode, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string clichePartNumber)
        {
            return View();
        }
    }
}
