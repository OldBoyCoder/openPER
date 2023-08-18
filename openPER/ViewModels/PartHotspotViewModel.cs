
namespace openPER.ViewModels
{
    public class PartHotspotViewModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double XPercent { get; set; }
        public double YPercent { get; set; }
        public double WidthPercent { get; set; }
        public double HeightPercent { get; set; }

        public string Link;
        //left: @(h.XPercent)%; top: @(h.YPercent)%; width: @(h.WidthPercent)%; height:@(h.HeightPercent)% display: block; position: absolute
        //GroupCode=h.Link.Substring(0,3), SubGroupCode=h.Link.Substring(3, 2), SubSubGroupCode=h.Link.Substring(6,3)
        public string LinkGroupCode => Link.Length >= 3 ? Link[..3] : "";
        public string LinkSubGroupCode => Link.Length >= 5 ? Link.Substring(3, 2) : "";
        public string LinkSubSubGroupCode => Link.Length >= 9 ? Link.Substring(6, 3) : "";

        public string PositionHtml
        {
            get
            {
                var rc =
                    $"left: {XPercent}%; top: {YPercent}%; width: {WidthPercent}%; height:{HeightPercent}%; display: block; position: absolute";

                return rc;
            }
        }

        public string Key => $"{X}:{Y}:{Width}:{Height}";
        public string TooltipText { get; set; }
        public string LinkDescription { get; set; }
        public int TableOrder { get; internal set; }
    }
}
