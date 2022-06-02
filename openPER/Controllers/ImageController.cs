using Microsoft.AspNetCore.Mvc;
using System.Linq;
using openPER.Interfaces;
using System;

namespace openPER.Controllers
{
    public class ImageController : Controller
    {
        private IVersionedRepository _rep;
        private IVersionedImageRespository _imageRep;
        public ImageController(IVersionedRepository repository, IVersionedImageRespository imageRep)
        {
            _rep = repository;
            _imageRep = imageRep;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("Image/{ReleaseCode}/{Model}")]
        public ActionResult ModelImage(int releaseCode, string model)
        {
            return File(_imageRep.GetImageForCatalogue(releaseCode, model), "image/png");
        }


    }
}
