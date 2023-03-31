using System.Data;
using System.Xml.Serialization;
using PdfSharpCore.Drawing;

namespace openPERPrinting;

internal class Table
{
    public List<TableColumn> Columns { get; set; } = new();
    public RenderFont HeadingFont { get; set; }
    public XPen GridPen { get; set; }

    public bool Showheading { get; set; }
    private DrawingsDocument _doc;
    private double _currentY;
    private double _totalWidth;
    private static readonly double _tablePadding = 1.0;
    private static readonly double _tableLineWidth = 1.0;
    private static readonly double _picturePadding = 10.0;
    public void Start(DrawingsDocument drawingsDocument, double startY)
    {
        _doc = drawingsDocument;
        _currentY = startY;
        // Work out column widths
        if (Columns.Any(x => x.Width <0))
        {
            // We have a filler column to populate
            // Work out the missing value
            var w = _doc.PageWidth;
            w -= Columns.Where(x => x.Width >0.0).Sum(x => x.Width);
            Columns.First(x => x.Width <0).Width = w;
        }
        _totalWidth = Columns.Sum(x => x.Width);
        DrawTableHeading();
    }

    private void DrawTableHeading()
    {
        _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY, _doc.StartX + _totalWidth, _currentY);
        if (Showheading)
        {
            var rowHeight = this.HeadingFont.Height(_doc.Gfx) + (2 * _tablePadding) + _tableLineWidth;
            // Draw headers
            _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY, _doc.StartX, _currentY + rowHeight);
            var xx = _doc.StartX;
            foreach (var column in Columns)
            {
                _doc.Gfx.DrawStringCentre(column.Heading, xx + _tableLineWidth + _tablePadding,
                    _currentY + _tableLineWidth + _tablePadding,
                    column.Width - (2 * _tablePadding) - _tableLineWidth, HeadingFont);
                // Draw line at right hand edge
                xx += column.Width;
                _doc.Gfx.DrawLine(GridPen, xx, _currentY, xx, _currentY + rowHeight);
            }

            _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY + rowHeight, _doc.StartX + _totalWidth,
                _currentY + rowHeight);
            _currentY += rowHeight;
        }
        // Other wise we just draw a line across the page to start the table
        _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY, _doc.StartX + _totalWidth, _currentY);
    }

    public async Task AddRow(params string[] data)
    {
        // Adding a row of data
        if (data.Length != Columns.Count)
            throw new InvalidDataException("Mismatch in data column versus headers");
        var dataLines = FitTableItemsIntoColumns(data, out var rowHeight);
        // Draw vertical lines
        // Initial line is standard
        if (_currentY + rowHeight > _doc.PageHeight)
        {
            _currentY = await _doc.NewPage();
            DrawTableHeading();
        }
        _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY, _doc.StartX, _currentY +rowHeight);
        var xx = _doc.StartX;
        foreach (var column in Columns)
        {
            // Draw line at right hand edge
            xx += column.Width;
            _doc.Gfx.DrawLine(GridPen, xx, _currentY, xx, _currentY+rowHeight);
        }

        var yy = _currentY + _tableLineWidth + _tablePadding;
        for (int y = 0; y < dataLines.Count; y++)
        {
            xx = _doc.StartX;
            for (int x = 0; x < dataLines[y].Count; x++)
            {
                var textY = yy + y * (Columns[x].Font.Height(_doc.Gfx) + 2 * _tablePadding + _tableLineWidth);
                if (Columns[x].Alignment == XStringAlignment.Far)
                    _doc.Gfx.DrawStringRight(dataLines[y][x], xx + _tableLineWidth + _tablePadding, textY,
                        Columns[x].Width - (2 * _tablePadding) - _tableLineWidth, Columns[x].Font);
                if (Columns[x].Alignment == XStringAlignment.Near)
                    _doc.Gfx.DrawString(dataLines[y][x], xx + _tableLineWidth + _tablePadding, textY, Columns[x].Font);
                if (Columns[x].Alignment == XStringAlignment.Center)
                    _doc.Gfx.DrawStringCentre(dataLines[y][x], xx + _tableLineWidth + _tablePadding, textY,
                        Columns[x].Width - (2 * _tablePadding) - _tableLineWidth, Columns[x].Font);
                xx += Columns[x].Width;
            }
        }
        _doc.Gfx.DrawLine(GridPen, _doc.StartX, _currentY+rowHeight, _doc.StartX+_totalWidth, _currentY + rowHeight);
        _currentY += rowHeight;
    }

    public double End()
    {
        return _currentY;
    }

    private List<List<string>> FitTableItemsIntoColumns(string[] fields, out double rowHeight)
    {
        var maxLines = 1;
        var rc = new List<List<string>>();
        var widths = Columns.Select(x => x.Width - (2.0 * _tablePadding) - _tableLineWidth).ToArray();
        // so work out if each column fits in its width, if not work out how many rows are needed
        // then return a 2D array of the split items
        for (var i = 0; i < widths.Length; i++)
        {
            fields[i] = fields[i].Replace("\r", "");
            fields[i] = fields[i].Replace("\n", "");
            var lines = BreakGroupIntoLines(widths[i], Columns[i].Font, _doc.Gfx, fields[i]);
            if (lines.Count > maxLines)
                maxLines = lines.Count;
        }

        for (var i = 0; i < maxLines; i++)
        {
            rc.Add(new List<string>());
            for (var j = 0; j < widths.Length; j++)
            {
                var lines = BreakGroupIntoLines(widths[j], Columns[j].Font, _doc.Gfx, fields[j]);
                rc[i].Add(lines.Count - 1 < i ? "" : lines[i]);
            }
        }

        rowHeight = double.MinValue;
        for (var i = 0; i < widths.Length; i++)
        {
            var nonBlanksRows = rc.Count(row => row[i] != "");
            var height = nonBlanksRows * (Columns[i].Font.Height(_doc.Gfx) + (2 * _tablePadding) + _tableLineWidth);
            if (height > rowHeight)
                rowHeight = height;
        }

        return rc;
    }
    private List<string> BreakGroupIntoLines(double initialWidth, RenderFont font, XGraphics gfx, string group)
    {
        //group = group.Replace("\r", "");
        //group = group.Replace("\n", "");
        var parts = StringIntoWords(group);
        var lines = new List<string>();
        var text = "";
        while (parts.Count > 0)
        {
            var oldText = text;
            text += parts[0];
            //text = text.Trim();
            var textWidth = gfx.MeasureString(text, font.Font);
            if (textWidth.Width > initialWidth)
            {
                lines.Add(oldText);
                // Need to write out old text
                text = parts[0];
            }

            parts.RemoveAt(0);
        }

        lines.Add(text);
        return lines;
    }

    private static List<string> StringIntoWords(string text)
    {
        var parts = new List<string>();
        var word = "";
        foreach (var c in text)
        {
            word += c;
            if (c is ' ' or '+' or ',')
            {
                parts.Add(word);
                word = "";
            }
        }
        if (word != "") parts.Add(word);
        return parts;
    }

}