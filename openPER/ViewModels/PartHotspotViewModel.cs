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
        public int TableOrder { get; set; }
        public string PartNumber { get; set; }
        //left: @(h.XPercent)%; top: @(h.YPercent)%; width: @(h.WidthPercent)%; height:@(h.HeightPercent)% display: block; position: absolute
        public string PositionHtml
        {
            get
            {
                var rc =
                    $"left: {XPercent}%; top: {YPercent}%; width: {WidthPercent}%; height:{HeightPercent}% display: block; position: absolute";

                return rc;
            }
        }

    }
}
