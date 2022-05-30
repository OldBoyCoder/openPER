using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using System;
using System.Threading;

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
        // GET: TableController/Details/5
        [Route("Table/Makes")]
        public ActionResult Makes()
        {
            return View(rep.GetAllMakes());
        }
        [Route("Table/Models/{Make}")]
        public ActionResult Models(string make)
        {
            return View(rep.GetAllModels(make));
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
        [Route("Table/Image/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        public ActionResult Image(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        {
            // TODO get rid of hard coded file name
            var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 20\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
            var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

            return File(GetImageFromNaFile(fileName, imageName, false), "image/png");
        }
        [Route("Table/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        public ActionResult Thumbnail(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        {
            // TODO get rid of hard coded file name
            var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 20\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
            var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

            return File(GetImageFromNaFile(fileName, imageName, true), "image/png");
        }
        private static byte[] GetImageFromNaFile(string fileName, string imageName, bool wantThumbail)
        {
            var reader = new System.IO.BinaryReader(System.IO.File.OpenRead(fileName));
            reader.ReadInt16();
            Int32 numberOfEntries = reader.ReadInt16();
            for (int i = 0; i < numberOfEntries; i++)
            {
                reader.ReadInt16(); // image index
                byte[] imageNameBytes = reader.ReadBytes(10);
                string entry = System.Text.Encoding.ASCII.GetString(imageNameBytes);
                Int32 mainImageStart = reader.ReadInt32();
                Int32 mainImageLength = reader.ReadInt32();
                Int32 thumbImageStart = reader.ReadInt32();
                Int32 thumbImageLength = reader.ReadInt32();
                if (entry == imageName)
                {
                    if (wantThumbail)
                    {
                        reader.BaseStream.Seek(thumbImageStart, System.IO.SeekOrigin.Begin);
                        return reader.ReadBytes(thumbImageLength);

                    }
                    else
                    {
                        reader.BaseStream.Seek(mainImageStart, System.IO.SeekOrigin.Begin);
                        return reader.ReadBytes(mainImageLength);

                    }
                }
            }
            return null;
        }
    }
}
