using PdfSharpCore.Drawing;

namespace openPERPrinting;

public class RenderFont
{
    internal readonly XFont Font;

    internal RenderFont(XFont xFont)
    {
        Font = xFont;
    }

    internal double Height(XGraphics gfx)
    {
        return gfx.MeasureString("Ay", Font).Height + 1;
    }

}