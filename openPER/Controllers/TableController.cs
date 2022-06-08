using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class TableController : Controller
    {
        private readonly IVersionedRepository _rep;
        private readonly IMapper _mapper;
        public TableController(IVersionedRepository repository, IMapper mapper)
        {
            _rep = repository;
            _mapper = mapper;   
        }

        // GET: TableController/Details/5
        [Route("Table/Details/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Drawing}")]
        public ActionResult Details(int releaseCode,string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawing)
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var x = _rep.GetTable(releaseCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawing, language);
            x.CatalogueCode = catalogueCode;
            x.GroupCode = groupCode;
            x.SubGroupCode = subGroupCode;
            x.SubSubGroupCode = subSubGroupCode;
            // Get Make code properly
            x.MakeCode = makeCode;
            x.ModelCode = modelCode;
            var vm = _mapper.Map<TableModel, TableViewModel>(x);    
            return View(x);
        }
        //[Route("Table/Catalogues/{Make}/{Model}")]
        //public ActionResult Catalogues(string make, string model)
        //{
        //    var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
        //    return View(_rep.GetAllCatalogues(make, model, language));
        //}
        //[Route("Table/Groups/{Make}/{Model}/{Catalogue}")]
        //public ActionResult Groups(string make, string model, string catalogue)
        //{
        //    var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);

        //    return View(_rep.GetGroupsForCatalogue(catalogue, language));
        //}
        //[Route("Table/Image/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroupCode}/{Drawing}")]
        //public ActionResult Image(string make, string model, string catalogue, int group, int subgroup, int sgsCode, int drawing)
        //{
        //    // TODO get rid of hard coded file name
        //    var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 18\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
        //    var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

        //    return File(GetImageFromNaFile(fileName, imageName, false), "image/png");
        //}
        //[Route("Table/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroupCode}/{Drawing}")]
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
        //    return File(GetImageForModel(model), "image/png");
        //}

    }
}
