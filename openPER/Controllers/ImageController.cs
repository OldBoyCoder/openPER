using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;

namespace openPER.Controllers
{
    public class ImageController : Controller
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly IVersionedRepository _repository;
        private readonly IVersionedImageRepository _imageRep;
        public ImageController(IVersionedRepository repository, IVersionedImageRepository imageRep)
        {
            _repository = repository;
            _imageRep = imageRep;
        }

        [Route("Image/ModelImage/{ReleaseCode}/{Make}/{Model}")]
        public ActionResult ModelImage(int releaseCode, string make, string model)
        {
            return File(_imageRep.GetImageForCatalogue(releaseCode, make, model), "image/png");
        }
        [Route("Image/{ReleaseCode}/{Make}/{Model}")]
        public ActionResult Drawing(int releaseCode, string make, string model)
        {
            return File(_imageRep.GetImageForCatalogue(releaseCode, make, model), "image/png");
        }
        //            <img src="@Url.Action("Drawing", "Image", new {Make=Model.MakeCode, Model=Model.ModelCode, Catalogue=Model.CatalogueCode, Group = Model.GroupCode, SubGroup = Model.SubGroupCode, Model.SubSubGroupCode, Drawing=Model.CurrentDrawing})">

        // TODO add release code to this route - need to create table view model first though
        [Route("Image/Drawing/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroup}/{Drawing}")]
        public ActionResult Drawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup, int drawing)
        {
            var x = _imageRep.GetImageForDrawing(18, make, model, catalogue, group, subgroup, subSubGroup, drawing);
            return File(x, "image/png");
        }
        // TODO add release code to this route - need to create table view model first though
        [Route("Image/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroup}/{Drawing}")]
        public ActionResult Thumbnail(string make, string model, string catalogue, int group, int subgroup, int subSubGroup, int drawing)
        {
            var x = _imageRep.GetThumbnailForDrawing(18, make, model, catalogue, group, subgroup, subSubGroup, drawing);
            return File(x, "image/png");
        }
        //{
        //    // TODO get rid of hard coded file name
        //    var fileName = System.IO.Path.Combine(@"C:\ePer installs\Release 18\SP.NA.00900.FCTLR", $"{make}{catalogue}.na");
        //    var imageName = $"{group}{subgroup.ToString("00")}{sgsCode.ToString("00")}{drawing.ToString("000")}";

        //    return File(GetImageFromNaFile(fileName, imageName, false), "image/png");
        //}


    }
}
