namespace VinSearcher
{
    public class VinResult
    {
        public string Mvs { get; set; }
        public string Chassis { get; set; }
        public string Organization { get; set; }
        public string Motor { get; set; }
        public string Date { get; set; }
        public string InteriorColour { get; set; }
        public string VIN { get; set; }
        public string Series { get; internal set; }
        public string Guide { get; internal set; }
        public string ShopEquipment { get; internal set; }
        public string ExteriorColour { get; internal set; }
        public string Options { get; internal set; }

        public string Marque
        {
            get { return Mvs[..3]; }
        }
        public string Model
        {
            get { return Mvs.Substring(3, 3); }
        }
        public string Version
        {
            get { return Mvs.Substring(6, 1); }
        }

        public string Caratt { get; internal set; }
    }
}
