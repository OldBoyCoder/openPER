using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;

namespace openPER.Controllers
{
    public class ImageController : Controller
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly IVersionedRepository _repository;
        private readonly IVersionedImageRespository _imageRep;
        public ImageController(IVersionedRepository repository, IVersionedImageRespository imageRep)
        {
            _repository = repository;
            _imageRep = imageRep;
        }

        [Route("Image/{ReleaseCode}/{Make}/{Model}")]
        public ActionResult ModelImage(int releaseCode,string make, string model)
        {
            return File(_imageRep.GetImageForCatalogue(releaseCode,make, model), "image/png");
        }


    }
}
