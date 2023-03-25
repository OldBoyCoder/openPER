using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class PrintController : Controller
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;

        public PrintController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var x = new openPERPrinting.PdfPrint(_rep);
            var fileStream = x.CreateDocument();
            return File(fileStream, "application/pdf", "MyDocument.PDF");
        }
    }
}
