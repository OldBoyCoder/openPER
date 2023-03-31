using PdfSharpCore.Drawing;

namespace openPERPrinting;

public static class XGraphicsHelpers
{
    public static void DrawHollowBox(this XGraphics gfx, XPen pen, double x1, double y1, double x2, double y2)
    {
        var pts = new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y1), new XPoint(x2, y2), new XPoint(x1, y2), new XPoint(x1, y1) };
        gfx.DrawLines(pen, pts);
    }

    public static double DrawString(this XGraphics gfx, string str, double x, double y, RenderFont font)
    {
        var rect = gfx.MeasureString(str, font.Font);
        gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, rect.Width, rect.Height),
            XStringFormats.CenterLeft);
        return rect.Height;
    }

    public static double DrawStringCentre(this XGraphics gfx, string str, double x, double y, double w, RenderFont font)
    {
        var rect = gfx.MeasureString(str, font.Font);
        gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, w, rect.Height), XStringFormats.TopCenter);
        return rect.Height;
    }
    public static double DrawStringCentre(this XGraphics gfx, string str, double x, double y, double w, double h, RenderFont font)
    {
        gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, w, h), XStringFormats.Center);
        return h;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    public static double DrawStringRight(this XGraphics gfx, string str, double x, double y, double w, RenderFont font)
    {
        var rect = gfx.MeasureString(str, font.Font);
        gfx.DrawString(str, font.Font, XBrushes.Black, new XRect(x, y, w, rect.Height), XStringFormats.CenterRight);
        return rect.Height;
    }

    private static async Task<XImage> FromUri(string uri)
    {
        using var httpClient = new HttpClient();
        var imageContent = await httpClient.GetByteArrayAsync(uri);
        using var imageBuffer = new MemoryStream(imageContent);
        return XImage.FromStream(() => imageBuffer);
    }

    public static async Task<double> DrawDrawingFromStream(this XGraphics gfx, string imagePath, double x1, double y1, double x2, double y2)
    {
        if (string.IsNullOrEmpty(imagePath)) return y2;
        double w;
        double h;

        var image = await FromUri(imagePath);
        var xFact = (x2 - x1) / image.PointWidth;
        var yFact = (y2 - y1) / image.PointHeight;
        if (xFact < yFact)
        {
            w = image.PointWidth * xFact;
            h = image.PointHeight * xFact;
        }
        else
        {
            w = image.PointWidth * yFact;
            h = image.PointHeight * yFact;


        }
        gfx.DrawImage(image, x1 + (x2 - x1) / 2 - w / 2, y1 + (y2 - y1) / 2 - h / 2, w, h);
        return y2;
    }
}