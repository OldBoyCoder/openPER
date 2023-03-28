using openPERRepositories.Interfaces;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;

namespace openPERPrinting
{
    public class PdfPrint
    {
        IRepository _rep;
        private PdfDocument _doc;
        private PdfPage _page;
        private XGraphics _gfx;
        private double _groupsWidth;
        private double _punchMargin;
        private double _littleGap;
        private long _pageNumber = 1;
        private double _contentsY;
        private string _lastGroup;
        private static RenderFont _tableFont = new(new XFont("Arial", 7, XFontStyle.Regular));
        private static RenderFont _titleFont = new(new XFont("Arial", 14, XFontStyle.Regular));
        private static RenderFont _contentsFont = new(new XFont("Arial", 10, XFontStyle.Regular));
        private static RenderFont _footerFont = new(new XFont("Arial", 6, XFontStyle.Regular));
        private double _workingWidth;
        private double _partsListTotalWidth;

        private readonly Dictionary<string, long> _tablesPages = new Dictionary<string, long>();
        private readonly Dictionary<string, long> _groupsPages = new Dictionary<string, long>();


        private PrintingOptions _options;

        private static XStringFormat[] _partsListAlignments =
        {
            XStringFormats.TopLeft, XStringFormats.TopRight, XStringFormats.TopLeft, XStringFormats.TopLeft,
            XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopLeft
        };

        private static XStringFormat[] _legendListAlignments = { XStringFormats.TopLeft, XStringFormats.TopLeft };

        private static XStringFormat[] _contentAlignments =
            {XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopRight};

        private List<double> _legendListWidths;
        private List<double> _contentWidths;
        private List<double> _partsListWidths;

        public PdfPrint(IRepository rep)
        {
            _rep = rep;
        }

        private PdfPage NewPage()
        {
            var p = _doc.AddPage();
            _gfx = XGraphics.FromPdfPage(p);
            p.Orientation = PageOrientation.Portrait;
            p.Size = (_options.PaperSize == PaperSize.A4) ? PageSize.A4 : PageSize.Letter;
            return p;

        }
        public async Task<Stream> CreateDocument(PrintingOptions options)
        {
            _options = options;
            _doc = new PdfDocument();
            _page = NewPage();

            _punchMargin = GetWidth(_page, 8);
            _littleGap = GetWidth(_page, 0.5);
            _workingWidth = GetWidth(_page, 100) - _punchMargin - _littleGap;
            _partsListWidths = GenerateColumnWidths(0.05, 0.12, 0.31, 0.15, 0.12, 0.15, 0.05, 0.05);
            _legendListWidths = GenerateColumnWidths(0.2, 0.8);
            _contentWidths = GenerateColumnWidths(0.2, 0.7, 0.1);
            _partsListTotalWidth = _partsListWidths.Sum(x => x) + (_partsListWidths.Count - 1) * _littleGap;

            await DrawTitlePage();
            await AddGroups();
            var ms = new MemoryStream();
            _doc.Save(ms, false);
            return ms;
        }
        public async Task AddGroups()
        {
            _lastGroup = "";
            var outputStream = new MemoryStream(16 * 1024 * 1024);

            foreach (var drawing in _options.Catalogue.Drawings.OrderBy(x => x.GroupCode).ThenBy(x => x.SubGroupCode).ThenBy(x => x.SubSubGroupCode).ThenBy(x => x.Variant).ThenBy(x => x.Revision))
            {
                _page = NewPage();
                var y = GetHeight(_page, 10);
                y += DrawStringCentre(
                    $"{drawing.GroupCode:000}{drawing.SubGroupCode:00}/{drawing.SubSubGroupCode:00}",
                    _punchMargin, y, _page.Width - _punchMargin,
                    _contentsFont);
                y += DrawStringCentre(
                    $"{drawing.ImagePath}",
                    _punchMargin, y, _page.Width - _punchMargin,
                    _contentsFont);
                y += await DrawDrawingFromStream(drawing.ImagePath, y);
//                _groupsPages.Add(group.Code.ToString("000"), _pageNumber);
                // Add section header page
                //DrawSectionHeaderPageHolder();
                var headerPage = _page;
                //AddTables(catalogue, group);
                // we can go back and add the page numbers to the header page
                //DrawSectionHeaderPage(headerPage, group);
            }

            // Go back and add contents to main page
            //AddMainPageContents(catalogue, titleDoc);

        }

        private List<double> GenerateColumnWidths(params double[] widths)
        {
            var returnList = new List<double>();
            for (var i = 0; i < widths.Length; i++)
                if (i < widths.Length - 1)
                    returnList.Add(_workingWidth * widths[i] - _littleGap);
                else
                    returnList.Add(_workingWidth * widths[i] - _littleGap);

            return returnList;
        }
        private static double GetWidth(PdfPage page, double portion)
        {
            return portion / 100.0 * page.Width;
        }

        private static double GetHeight(PdfPage page, double portion)
        {
            return portion / 100.0 * page.Height;
        }
        private async Task DrawTitlePage()
        {
            AddPageFooter();

            var y = GetHeight(_page, 10);
            y += DrawStringCentre("Parts list", _punchMargin, y, _page.Width - _punchMargin, _titleFont);
            y += DrawStringCentre($"{_options.Catalogue.Code} - {_options.Catalogue.Description}", _punchMargin, y,
                _page.Width - _punchMargin, _titleFont);
            y += DrawStringCentre($"Produced on {DateTime.Now:G}", _punchMargin, y, _page.Width - _punchMargin,
                _titleFont);
            y += DrawStringCentre($"Using version {Assembly.GetExecutingAssembly().GetName().Version}",
                _punchMargin, y, _page.Width - _punchMargin, _titleFont);
            y = await DrawDrawingFromStream(_options.Catalogue.ImagePath, y + 20);
            _contentsY = y;
        }

        private void AddPageFooter()
        {
            var y = GetHeight(_page, 95);
            DrawStringCentre(
                "Source data is Copyright(c) 2011, Fiat Group Automobiles.  Code to produce this PDF is Copyright(c) 2021, Chris Reynolds.",
                _punchMargin, y, _page.Width - _punchMargin * 2, _footerFont);
            DrawStringRight($"Page: {_pageNumber}", _punchMargin, y, _page.Width - _punchMargin * 2, _footerFont);
            //DrawDrawingFromFile(@"c:\temp\ff_logo.png", y, _punchMargin);
            _pageNumber++;
        }

        private double DrawString(string str, double x, double y, RenderFont font)
        {
            var rect = _gfx.MeasureString(str, font.Font);
            _gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, rect.Width, rect.Height),
                XStringFormats.TopLeft);
            return rect.Height;
        }

        private double DrawStringCentre(string str, double x, double y, double w, RenderFont font)
        {
            var rect = _gfx.MeasureString(str, font.Font);
            _gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, w, rect.Height), XStringFormats.TopCenter);
            return rect.Height;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private double DrawStringRight(string str, double x, double y, double w, RenderFont font)
        {
            var rect = _gfx.MeasureString(str, font.Font);
            _gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, w, rect.Height), XStringFormats.TopRight);
            return rect.Height;
        }

        public static async Task<XImage> FromUri(string uri)
        {
            using var httpClient = new HttpClient();
            var imageContent = await httpClient.GetByteArrayAsync(uri);
            using var imageBuffer = new MemoryStream(imageContent);
            return XImage.FromStream(() => imageBuffer);

            //Do something with image
        }
        private async Task<double> DrawDrawingFromStream(string imagePath, double startY)
        {
            if (string.IsNullOrEmpty(imagePath)) return startY;
            var image = await FromUri(imagePath);

            var xx = GetWidth(_page, 80) - (_groupsWidth + _littleGap + _punchMargin);
            var yy = xx;
            if (image.PixelWidth > image.PixelHeight)
                yy = image.PixelHeight * xx / image.PixelWidth;
            else
                xx = image.PixelWidth * yy / image.PixelHeight;
            // Don't make it too far down the page
            if (yy > _page.Height / 2)
            {
                var f = (_page.Height / 2) / yy;
                xx *= f;
                yy = _page.Height / 2;
            }
            var centreX = _punchMargin + GetWidth(_page, 80) / 2.0;
            _gfx.DrawImage(image, centreX - xx / 2, startY + _littleGap, xx, yy);
            return startY + _littleGap + yy + _littleGap;
        }

        private double DrawDrawingFromFile(string imagePath, double startY, double x)
        {
            if (string.IsNullOrEmpty(imagePath)) return startY;
            var image = XImage.FromFile(imagePath);

            _gfx.DrawImage(image, x, startY, 57, 16);
            return startY + image.PixelHeight;

        }


    }



}