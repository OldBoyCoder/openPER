using openPERRepositories.Interfaces;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;

namespace openPERPrinting
{
    internal class TableColumn
    {
        public string Heading { get; set; }
        public double Width { get; set; }
        public RenderFont Font { get; set; }
        public XStringAlignment Alignment { get; set; }

        public TableColumn(string heading, RenderFont font, double width, XStringAlignment alignment)
        {
            Heading = heading;
            Font = font;
            Width = width;
            Alignment = alignment;
        }
    }

    public class PdfPrint
    {
        private DrawingsDocument drawDoc;
        IRepository _rep;
        private double _contentsY;
        private double _workingWidth;
        private double _partsListTotalWidth;

        private readonly Dictionary<string, long> _tablesPages = new();
        private readonly Dictionary<string, long> _groupsPages = new();


        private static readonly XStringFormat[] _partsListAlignments =
        {
            XStringFormats.TopLeft, XStringFormats.TopRight, XStringFormats.TopLeft, XStringFormats.TopLeft,
            XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopLeft
        };

        private static readonly XStringFormat[] _legendListAlignments = { XStringFormats.TopLeft, XStringFormats.TopLeft };

        private static readonly XStringFormat[] _contentAlignments =
            {XStringFormats.TopLeft, XStringFormats.TopLeft, XStringFormats.TopRight};

        private List<double> _legendListWidths;
        private List<double> _contentWidths;

        public PdfPrint(IRepository rep)
        {
            _rep = rep;
        }


        public async Task<MemoryStream> CreateDocument(PrintingOptions options)
        {
            var doc = new DrawingsDocument(options);
            var ms = await doc.CreatePdf();

            return ms;
        }

        private static double GetWidth(PdfPage page, double portion)
        {
            return portion / 100.0 * page.Width;
        }

        private static double GetHeight(PdfPage page, double portion)
        {
            return portion / 100.0 * page.Height;
        }

    }



}