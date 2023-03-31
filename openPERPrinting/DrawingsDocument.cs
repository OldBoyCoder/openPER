using openPERModels;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace openPERPrinting;

internal class DrawingsDocument
{
    private static XPen _tablegridPen = new XPen(XColors.Black, 0.5);
    private const double RightMargin = 30;
    private const double LeftMargin = 30;
    private const double TopMargin = 30;
    private const double BottomMargin = 30;
    private static readonly double _picturePadding = 10.0;
    private const string logoPath = "https://cdn.fiatforum.com/data/assets/logo/logo.png";
    private PdfDocument _doc;
    private PdfPage _page;
    public XGraphics Gfx;
    private double _punchMargin;
    private long _pageNumber = 1;
    public static readonly RenderFont _tableFont = new(new XFont("Arial", 7, XFontStyle.Regular));
    public static readonly RenderFont _smallTableFont = new(new XFont("Arial", 5, XFontStyle.Regular));
    public static readonly RenderFont _tableFontBold = new(new XFont("Arial", 7, XFontStyle.Bold));
    public static readonly RenderFont _h1Font = new(new XFont("Arial", 12, XFontStyle.Bold));
    public static readonly RenderFont _footerFont = new(new XFont("Arial", 6, XFontStyle.Regular));
    public static readonly RenderFont _headerFont = new(new XFont("Arial", 10, XFontStyle.Bold));
    public double PageWidth => (_page.Width - LeftMargin - RightMargin);
    public double PageHeight => (_page.Height - TopMargin - BottomMargin);

    public double StartX => LeftMargin;
    public double StartY => TopMargin;
    private PrintingOptions _options;
    private DrawingKeyModel _currentDrawing;

    private static string[] _partsHeadings =
        { "", "Part Numb.", "Description", "Compatibility", "Modif.", "Qty", "Notes", "Colour", "" };

    public DrawingsDocument(PrintingOptions options)
    {
        _options = options;
    }
    public async Task<double> NewPage()
    {
        _page = _doc.AddPage();
        Gfx = XGraphics.FromPdfPage(_page);
        _page.Orientation = PageOrientation.Portrait;
        _page.Size = (_options.PaperSize == PaperSize.A4) ? PageSize.A4 : PageSize.Letter;
        var y = await PageHeader(_currentDrawing);
        y += 10;
        AddPageFooter();
        return y;

    }

    private async Task<double> PageHeader(DrawingKeyModel drawing)
    {
        // Three parts
        // Logo -- Static title and date -- Drawing reference
        // height is 35 points
        // Draw full width box with vertical lines at the divisions
        var x1 = StartX;
        var x2 = StartX + PageWidth / 3.0;
        var x3 = StartX + (2.0 * PageWidth / 3.0);
        var x4 = StartX + PageWidth;
        var y1 = StartY;
        var y2 = StartY + 35;
        var h = (y2 - y1);
        Gfx.DrawLine(XPens.Black, x1, y1, x4, y1);
        Gfx.DrawLine(XPens.Black, x1, y2, x4, y2);
        Gfx.DrawLine(XPens.Black, x1, y1, x1, y2);
        Gfx.DrawLine(XPens.Black, x2, y1, x2, y2);
        Gfx.DrawLine(XPens.Black, x3, y1, x3, y2);
        Gfx.DrawLine(XPens.Black, x4, y1, x4, y2);
        Gfx.DrawStringCentre("Spare Parts Catalogue", x2, y1, x3 - x2, h / 2, DrawingsDocument._headerFont);
        Gfx.DrawStringCentre(DateTime.Now.ToShortDateString(), x2, y1 + (h / 2), x3 - x2, h / 2,
            DrawingsDocument._headerFont);
        var codeString =
            $"{drawing.MakeCode}.{drawing.CatalogueCode}.{drawing.GroupCode:000}{drawing.SubGroupCode:00}/{drawing.SubSubGroupCode:00}.{drawing.Variant:00}.{drawing.Revision:00}";
        Gfx.DrawStringCentre(codeString, x3, y1, x4 - x3, y2 - y1, DrawingsDocument._headerFont);
        var w = x2 - x1;
        await Gfx.DrawDrawingFromStream(logoPath, x1 + w / 8, y1 + h / 8, x2 - w / 8, y2 - h / 8);
        var y = y2 + 10;
        var text =
            $"{drawing.GroupCode:000}{drawing.SubGroupCode:00}/{drawing.SubSubGroupCode:00} - {drawing.TableDescription}";
        y += Gfx.DrawString(text, StartX, y, DrawingsDocument._h1Font);
        return y;
    }

    private async Task<double> AddDrawingImage(DrawingKeyModel drawing, double startY)
    {
        var y1 = startY + 10;
        var y2 = y1 + 230;
        Gfx.DrawHollowBox(XPens.Black, StartX, y1, StartX + PageWidth, y2);
        await Gfx.DrawDrawingFromStream(drawing.ImagePath, StartX + _picturePadding, y1 + _picturePadding, StartX + PageWidth - _picturePadding, y2 - _picturePadding);
        return y2;
    }
    public async Task<double> AddDrawingHeader(DrawingKeyModel drawing, double y)
    {
        var drawingHeaderTable = new Table
        {
            GridPen = _tablegridPen
        };
        drawingHeaderTable.Columns.Add(new TableColumn("", _tableFont, 100, XStringAlignment.Far));
        drawingHeaderTable.Columns.Add(new TableColumn("", _tableFont, double.MinValue, XStringAlignment.Near));
        drawingHeaderTable.Start(this, y);
        await drawingHeaderTable.AddRow("Catalogue", $"{drawing.CatalogueCode} - {drawing.CatalogueDescription}");
        await drawingHeaderTable.AddRow("Category", $"[{drawing.GroupCode:000}] {drawing.GroupDescription} / [{drawing.GroupCode:000}{drawing.SubGroupCode:00}] {drawing.SubGroupDescription}");
        await drawingHeaderTable.AddRow("Var.", $"{drawing.Variant} - {drawing.VariantPattern}");
        await drawingHeaderTable.AddRow("Rev.", $"{drawing.Revision} - {drawing.RevisionModifications}");
        return drawingHeaderTable.End();
    }
    private void AddPageFooter()
    {
        var y = 800;
        Gfx.DrawStringCentre(
            "Source data is Copyright(c) 2011, Fiat Group Automobiles.  Code to produce this PDF is Copyright(c) 2021, Chris Reynolds.", StartX, y, PageWidth, DrawingsDocument._footerFont);
        Gfx.DrawStringRight($"Page: {_pageNumber}", StartX, y, PageWidth, DrawingsDocument._footerFont);
        _pageNumber++;
    }

    public async Task<MemoryStream> CreatePdf()
    {
        _doc = new PdfDocument();
        foreach (var drawing in _options.Catalogue.Drawings)
        {
            _currentDrawing = drawing;
            var y = await NewPage();
            y = await AddDrawingHeader(drawing, y);
            y = await AddDrawingImage(drawing, y);
            y += 10; // Small gap before parts list
            y = await AddPartsList(drawing, y);
            y += 10; // Small gap before legend table
            y = await AddLegend(drawing, y);
        }
        var ms = new MemoryStream();
        _doc.Save(ms, false);
        return ms;

    }

    private async Task<double> AddLegend(DrawingKeyModel drawing, double y)
    {
        var legendTable = new Table
        {
            Showheading = true,
            HeadingFont = _tableFontBold,
            GridPen = _tablegridPen
        };
        legendTable.Columns.Add(new TableColumn("Legend", _tableFont, 60, XStringAlignment.Near));
        legendTable.Columns.Add(new TableColumn("", _tableFont, double.MinValue, XStringAlignment.Near));
        legendTable.Start(this, y);
        //var vv = openPERHelpers.PatternMatchHelper.GetSymbolsFromPattern(v, out var newPattern);
        //foreach (var symbol in vv)
        //{
        //    await legendTable.AddRow(symbol.Key, symbol.Value.ToString());
        //}
        foreach (var modification in drawing.Modifications)
        {
            await legendTable.AddRow(modification.Code.ToString(), modification.Description);
        }

        var colours = new List<ColourModel>();
        var v = drawing.VariantPattern;

        foreach (var part in drawing.Parts)
        {
            if (part.Colours != null)
                colours.AddRange(part.Colours);
        }
        foreach (var colour in colours.DistinctBy(x=>x.Code).OrderBy(x => x.Code))
        {
            await legendTable.AddRow(colour.Code, colour.Description);
        }
        return legendTable.End();
    }

    private async Task<double> AddPartsList(DrawingKeyModel drawing, double y)
    {
        //private static readonly double[] _partsListWidths = { 16.0, 60.0, 120.0, 120.0, 35.0, 20.0, 50.0, 50.0, 45.0 };
        var partsTable = new Table
        {
            Showheading = true,
            HeadingFont = _tableFontBold,
            GridPen = _tablegridPen
        };
        partsTable.Columns.Add(new TableColumn("", _tableFont, 16, XStringAlignment.Far));
        partsTable.Columns.Add(new TableColumn("Part Numb.", _tableFont, 60, XStringAlignment.Far));
        partsTable.Columns.Add(new TableColumn("Description", _tableFont, 120, XStringAlignment.Near));
        partsTable.Columns.Add(new TableColumn("Compatibility", _smallTableFont, 120, XStringAlignment.Near));
        partsTable.Columns.Add(new TableColumn("Modif.", _tableFont, 35, XStringAlignment.Near));
        partsTable.Columns.Add(new TableColumn("Qty", _tableFont, 20, XStringAlignment.Near));
        partsTable.Columns.Add(new TableColumn("Notes", _tableFont, 50, XStringAlignment.Near));
        partsTable.Columns.Add(new TableColumn("Colour", _tableFont, double.MinValue, XStringAlignment.Near));
        partsTable.Start(this, y);
        foreach (var part in drawing.Parts)
        {
            await partsTable.AddRow(part.TableOrder.ToString(), part.PartNumber, part.Description + " " + part.FurtherDescription, part.Compatibility, "Mods", part.Quantity, part.Notes, part.Colour);
        }
        return partsTable.End();
    }
}