using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using System;

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
            var x = rep.GetTable(make, model, catalogue, group, subgroup, sgsCode, drawing, "3");
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
            return View(rep.GetAllCatalogues(make, model));
        }
        [Route("Table/Image/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        public ActionResult Image(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        {
            // TODO get rid of hard coded file name
            var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 20\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
            var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

            return File(GetImageFromNaFile(fileName, imageName), "image/png");
        }
        [Route("Table/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SgsCode}/{Drawing}")]
        public ActionResult Thumbnail(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        {
            // TODO get rid of hard coded file name
            var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 20\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
            var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

            return File(GetThumbnailFromNaFile(fileName, imageName), "image/png");
        }
        private static byte[] GetImageFromNaFile(string fileName, string imageName)
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
                reader.ReadInt32();  // thumbnail start
                reader.ReadInt32(); // thumbnail size
                if (entry == imageName)
                {
                    reader.BaseStream.Seek(mainImageStart, System.IO.SeekOrigin.Begin);
                    return reader.ReadBytes(mainImageLength);
                }
            }
            return null;
        }
        private static byte[] GetThumbnailFromNaFile(string fileName, string imageName)
        {
            var reader = new System.IO.BinaryReader(System.IO.File.OpenRead(fileName));
            reader.ReadInt16();
            Int32 numberOfEntries = reader.ReadInt16();
            for (int i = 0; i < numberOfEntries; i++)
            {
                reader.ReadInt16(); // image index
                byte[] imageNameBytes = reader.ReadBytes(10);
                string entry = System.Text.Encoding.ASCII.GetString(imageNameBytes);
                reader.ReadInt32();  // main image start
                reader.ReadInt32(); // main image size
                Int32 ImageStart = reader.ReadInt32();
                Int32 ImageLength = reader.ReadInt32();
                if (entry == imageName)
                {
                    reader.BaseStream.Seek(ImageStart, System.IO.SeekOrigin.Begin);
                    return reader.ReadBytes(ImageLength);
                }
            }
            return null;
        }

    }
}
