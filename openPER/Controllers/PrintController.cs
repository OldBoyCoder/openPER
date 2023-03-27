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
                    ImagePath = catalogue.ImageName,
                    Groups = _rep.GetGroupsForCatalogue(catalogueCode, language, true)
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
                    ImagePath = catalogue.ImageName,
                    Groups = _rep.GetGroupsForCatalogue(catalogueCode, language, true)
                }
            };
            options.Catalogue.Groups = options.Catalogue.Groups.Where(x => x.Code == groupCode).ToList();
            options.Catalogue.Groups[0].SubGroups = options.Catalogue.Groups[0].SubGroups.Where(x => x.Code == subgroupCode).ToList();
            options.Catalogue.Groups[0].SubGroups[0].SubSubGroups = options.Catalogue.Groups[0].SubGroups[0]
                .SubSubGroups.Where(x => x.Code == subSubGroupCode).ToList();

            var fileStream = await x.CreateDocument(options);
            return File(fileStream, "application/pdf", "MyDocument.PDF");
        }

    }
}
