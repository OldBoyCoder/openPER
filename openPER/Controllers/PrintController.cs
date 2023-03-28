using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPERModels.PrintModels;
using openPERPrinting;
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
        [Route("{language}/Print/Catalogue/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Catalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode, string language, string mvs = "", string vin = "")
        {
            var x = new PdfPrint(_rep);
            var catalogue = _rep.GetCatalogue(makeCode, subMakeCode, modelCode, catalogueCode, language);

            var options = new PrintingOptions
            {
                PaperSize = PaperSize.A4,
                TitlePage = true,
                TableOfContents = true,
                PrintPagesPerSheet = PrintPagesPerSheet.SingleSided,
                Catalogue = new CataloguePrintViewModel
                {
                    Code = catalogueCode,
                    Description = catalogue.Description,
                    ImagePath = catalogue.ImageName
                    
                }
            };
            var fileStream = await x.CreateDocument(options);
            return File(fileStream, "application/pdf", "MyDocument.PDF");
        }
        [Route("{language}/Print/Catalogue/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SubSubGroup(string makeCode, string subMakeCode, string modelCode, string catalogueCode,int groupCode, int subgroupCode, int subSubGroupCode, string language, string mvs = "", string vin = "")
        {
            var x = new PdfPrint(_rep);
            var catalogue = _rep.GetCatalogue(makeCode, subMakeCode, modelCode, catalogueCode, language);

            var options = new PrintingOptions
            {
                PaperSize = PaperSize.A4,
                TitlePage = true,
                TableOfContents = true,
                PrintPagesPerSheet = PrintPagesPerSheet.SingleSided,
                Catalogue = new CataloguePrintViewModel
                {
                    Code = catalogueCode,
                    Description = catalogue.Description,
                    ImagePath = catalogue.ImageName
                    
                }
            };

            var fileStream = await x.CreateDocument(options);
            return File(fileStream, "application/pdf", "MyDocument.PDF");
        }

    }
}
