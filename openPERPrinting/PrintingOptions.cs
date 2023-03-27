using openPERModels.PrintModels;

namespace openPERPrinting;

public class PrintingOptions
{
    public PaperSize PaperSize { get; set; }
    public PrintPagesPerSheet PrintPagesPerSheet { get; set; }
    public bool TitlePage { get; set; }
    public bool TableOfContents { get; set; }
    public CataloguePrintViewModel Catalogue { get; set; }
    public string OptionsPattern { get; set; }

}