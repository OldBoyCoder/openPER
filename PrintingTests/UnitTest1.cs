using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Discovery;
using openPERModels;
using openPERModels.PrintModels;
using openPERPrinting;
using openPERRepositories.Interfaces;

namespace PrintingTests
{
    [TestClass]
    public class PdfPrintTests
    {
        private IConfiguration _configuration;
        private IRepository _rep;
        private PdfPrint _printer;
        private void SetUpObjects()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"TopLevelKey", "TopLevelValue"},
                {"Releases:1:Release", "84"},
                {"Releases:1:DbName", "Server=127.0.0.1;User ID=openPer;Password=openPer;IgnoreCommandTransaction=true;database=openPer"},
                {"Releases:1:CDNRoot", "https://data-84-eper.fiatforum.com/"},
                //...populate as needed for the test
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _rep = new openPERRepositories.Repositories.Release84Repository(_configuration);
            _printer = new PdfPrint(_rep);

        }
        [TestMethod]
        public async Task PrintBarchettaEngine()
        {
            SetUpObjects();
            var catalogue = _rep.GetCatalogue("F", "F", "BAR", "PK", "3");
            var drawings = _rep.GetDrawingDataForPrinting("F", "BAR", "PK", 551, 1,
                0, 0,DrawingsScope.SubSubGroup, "3");

            var options = SetPrintingOptions(catalogue, drawings);

            var result = await _printer.CreateDocument(options);
            await File.WriteAllBytesAsync(@"c:\temp\pdftest.pdf", result.ToArray());
            Assert.IsNotNull(result);

        }

        private static PrintingOptions SetPrintingOptions(CatalogueModel catalogue, List<DrawingKeyModel> drawings)
        {
            var options = new PrintingOptions
            {
                PaperSize = PaperSize.A4,
                TitlePage = true,
                TableOfContents = true,
                PrintPagesPerSheet = PrintPagesPerSheet.SingleSided,
                IncludeCliches = false,
                Catalogue = new CataloguePrintViewModel
                {
                    Code = catalogue.Code,
                    Description = catalogue.Description,
                    ImagePath = catalogue.ImageName,
                    Drawings = drawings
                }
            };
            return options;
        }

        [TestMethod]
        public async Task Print500DisplayUnits()
        {
            SetUpObjects();

            var catalogue = _rep.GetCatalogue("F", "F", "CIN", "83", "3");
            var drawings = _rep.GetDrawingDataForPrinting("F", "CIN", "83", 701, 33,
                10, 0, DrawingsScope.SubSubGroup, "3");

            var options = SetPrintingOptions(catalogue, drawings);

            var result = await _printer.CreateDocument(options);
            await File.WriteAllBytesAsync(@"c:\temp\pdftest.pdf", result.ToArray());
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task PrintWithLargePartVariants()
        {
            SetUpObjects();

            var catalogue = _rep.GetCatalogue("F", "F", "GIN", "3U", "3");
            var drawings = _rep.GetDrawingDataForPrinting("F", "GIN", "3U", 555, 1,
                0, 0, DrawingsScope.SubSubGroup, "3");

            var options = SetPrintingOptions(catalogue, drawings);

            var result = await _printer.CreateDocument(options);
            await File.WriteAllBytesAsync(@"c:\temp\pdftest.pdf", result.ToArray());
            Assert.IsNotNull(result);

        }

    }
}