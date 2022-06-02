using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult Details(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var x = rep.GetTable(make, model, catalogue, group, subgroup, sgsCode, drawing, language);
            x.MakeCode = make;
            x.ModelCode = model;
            x.CatalogueCode = catalogue;
            x.GroupCode = group;
            x.SubGroupCode = subgroup;
            x.SgsCode = sgsCode;
            return View(x);
        }
        [Route("Table/Catalogues/{Make}/{Model}")]
        public ActionResult Catalogues(string make, string model)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            return View(rep.GetAllCatalogues(make, model, language));
        }
        [Route("Table/Groups/{Make}/{Model}/{Catalogue}")]
        public ActionResult Groups(string make, string model, string catalogue)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

            return View(rep.GetGroupsForCatalogue(catalogue, language));
        }
        //[Route("Table/Image/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        //public ActionResult Image(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        //{
        //    // TODO get rid of hard coded file name
        //    var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 18\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
        //    var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

        //    return File(GetImageFromNaFile(fileName, imageName, false), "image/png");
        //}
        //[Route("Table/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        //public ActionResult Thumbnail(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        //{
        //    // TODO get rid of hard coded file name
        //    var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 18\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
        //    var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

        //    return File(GetImageFromNaFile(fileName, imageName, true), "image/png");
        //}
        //[Route("Table/{Model}")]
        //public ActionResult ModelImage(string model)
        //{
        //    return File(GetImageForCatalogue(model), "image/png");
        //}

    }
}
