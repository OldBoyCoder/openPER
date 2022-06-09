using Microsoft.AspNetCore.Mvc;
using openPERRepositories.Interfaces;

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

        [Route("Image/ModelImage/{ReleaseCode}/{Make}/{SubMake}/{Model}")]
        public ActionResult ModelImage(int releaseCode, string make, string subMake, string model)
        {
            return File(_imageRep.GetImageForModel(releaseCode, make, subMake, model), "image/png");
        }
        [Route("Image/SmallModelImage/{ReleaseCode}/{Make}/{SubMake}/{Model}")]
        public ActionResult SmallModelImage(int releaseCode, string make, string subMake, string model)
        {
            return File(_imageRep.GetSmallImageForModel(releaseCode, make, subMake, model), "image/png");
        }
        [Route("Image/CatalogueImage/{ReleaseCode}/{Make}/{SubMake}/{Model}/{Catalogue}")]
        public ActionResult CatalogueImage(int releaseCode, string make, string subMake, string model, string catalogue)
        {
            // For the catalogue image we need the map name which comes from the DB.
            // Not sure whether to pass a DB repository into the image repository or get it here
            // decided I didn't want a DB dependency in the image repository so get the map name here
            // Might find out I'm wrong when I did other versions
            var mapDetails = _repository.GetMapAndImageForCatalogue(releaseCode, make, subMake, model, catalogue);
            return File(_imageRep.GetImageForCatalogue(releaseCode, make, subMake, model, catalogue, mapDetails), "image/png");
        }
        [Route("Image/GroupImage/{ReleaseCode}/{Make}/{SubMake}/{Model}/{Catalogue}/{Group}")]
        public ActionResult CatalogueGroupImage(int releaseCode, string make, string subMake, string model, string catalogue, int group)
        {
            var mapDetails = _repository.GetMapForCatalogueGroup(releaseCode, make, subMake, model, catalogue, group);
            // TODO sort out imageName
            return File(_imageRep.GetImageForCatalogue(releaseCode, make, subMake, model, catalogue, mapDetails), "image/png");
        }
        [Route("Image/{ReleaseCode}/{Make}/{Model}")]
        public ActionResult Drawing(int releaseCode, string make,string subMakeCode, string model)
        {
            return File(_imageRep.GetImageForModel(releaseCode, make,subMakeCode, model), "image/png");
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
    }
}
