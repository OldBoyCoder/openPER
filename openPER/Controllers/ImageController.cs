using Microsoft.AspNetCore.Mvc;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class ImageController : Controller
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly IRepository _repository;
        private readonly IImageRepository _imageRep;
        public ImageController(IRepository repository, IImageRepository imageRep)
        {
            _repository = repository;
            _imageRep = imageRep;
        }

        [Route("Image/ModelImage/{Make}/{SubMake}/{Model}")]
        public ActionResult ModelImage(string make, string subMake, string model)
        {
            return File(_imageRep.GetImageForModel(make, subMake, model), "image/png");
        }
        [Route("Image/SmallModelImage/{Make}/{SubMake}/{Model}")]
        public ActionResult SmallModelImage(string make, string subMake, string model)
        {
            return File(_imageRep.GetSmallImageForModel(make, subMake, model), "image/png");
        }
        [Route("Image/CatalogueImage/{Make}/{SubMake}/{Model}/{Catalogue}")]
        public ActionResult CatalogueImage(string make, string subMake, string model, string catalogue)
        {
            // For the catalogue image we need the map name which comes from the DB.
            // Not sure whether to pass a DB repository into the image repository or get it here
            // decided I didn't want a DB dependency in the image repository so get the map name here
            // Might find out I'm wrong when I did other versions
            var mapDetails = _repository.GetMapAndImageForCatalogue(make, subMake, model, catalogue);
            return File(_imageRep.GetImageForCatalogue(make, subMake, model, catalogue, mapDetails), "image/png");
        }
        [Route("Image/GroupImage/{Make}/{SubMake}/{Model}/{Catalogue}/{Group}")]
        public ActionResult CatalogueGroupImage(string make, string subMake, string model, string catalogue, int group)
        {
            var mapDetails = _repository.GetMapForCatalogueGroup(make, subMake, model, catalogue, group);
            // TODO sort out imageName
            return File(_imageRep.GetImageForCatalogue(make, subMake, model, catalogue, mapDetails), "image/png");
        }
        [Route("Image/{Make}/{Model}")]
        public ActionResult Drawing(string make,string subMakeCode, string model)
        {
            return File(_imageRep.GetImageForModel(make,subMakeCode, model), "image/png");
        }
        //            <img src="@Url.Action("Drawing", "Image", new {Make=Model.MakeCode, Model=Model.ModelCode, Catalogue=Model.CatalogueCode, Group = Model.GroupCode, SubGroup = Model.SubGroupCode, Model.SubSubGroupCode, Drawing=Model.CurrentDrawing})">

        [Route("Image/Drawing/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroup}/{Variant}/{Revision}")]
        public ActionResult Drawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup, int variant, int revision)
        {
            var imageName = _repository.GetImageNameForDrawing(make, model, catalogue, group, subgroup,
                subSubGroup, variant, revision);
            var x = _imageRep.GetImageForDrawing(make, model, catalogue, group, subgroup, subSubGroup, variant, imageName);
            return File(x, "image/png");
        }
        [Route("Image/Thumbnail/{Make}/{Model}/{Catalogue}/{Group}/{Subgroup}/{SubSubGroup}/{Variant}/{Revision}")]
        public ActionResult Thumbnail(string make, string model, string catalogue, int group, int subgroup, int subSubGroup, int variant, int revision)
        {
            var imageName = _repository.GetImageNameForDrawing(make, model, catalogue, group, subgroup,
                subSubGroup, variant, revision);
            var x = _imageRep.GetThumbnailForDrawing(make, model, catalogue, group, subgroup, subSubGroup, variant, imageName);
            return File(x, "image/png");
        }

        [Route("Image/Thumbnail/{ClichePartNumber}/{ClichePartDrawingNumber}")]
        public ActionResult Thumbnail(string clichePartNumber, int clichePartDrawingNumber)
        {
            var imageName =
                _repository.GetImageNameForClicheDrawing(clichePartNumber, clichePartDrawingNumber);
            byte[] x = _imageRep.GetThumbnailForCliche(clichePartNumber, clichePartDrawingNumber, imageName);
            return File(x, "image/png");
        }
        [Route("Image/Drawing/{ClichePartNumber}/{ClichePartDrawingNumber}")]
        public ActionResult Drawing(string clichePartNumber, int clichePartDrawingNumber)
        {
            var imageName =
                _repository.GetImageNameForClicheDrawing(clichePartNumber, clichePartDrawingNumber);
            byte[] x = _imageRep.GetImageForCliche(clichePartNumber, clichePartDrawingNumber, imageName);
            return File(x, "image/png");
        }
    }
}
