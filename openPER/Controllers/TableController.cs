using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;

namespace openPER.Controllers
{
    public class TableController : Controller
    {
        private IRepository rep;
        public TableController(IRepository repository)
        {
            rep = repository;
        }
        // GET: TableController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TableController/Details/5
        [Route("Table/Details/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        public ActionResult Details(string make, string model, string catalogue, string group, string subgroup, string sgsCode, int drawing)
        {
            var x = rep.GetTable(make, model, catalogue, group, subgroup, "3");
            x.MakeCode = make;
            x.ModelCode = model;
            x.CatalogueCode = catalogue;
            x.GroupCode = group;
            x.SubGroupCode = subgroup;
            x.SgsCode = sgsCode;
            x.Parts = new System.Collections.Generic.List<TablePartViewModel>();
            var y = new TablePartViewModel();
            y.Description = "XCheese";
            x.Parts.Add(y);
            return View(x);
        }

    }
}
