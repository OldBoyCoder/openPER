using openPERModels;
using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class VinSearchResultViewModel
    {
        public string Mvs { get; set; }
        public string Chassis { get; set; }
        public string Organization { get; set; }
        public string Motor { get; set; }
        public string BuildDate { get; set; }
        public string InteriorColourCode { get; set; }
        public string InteriorColourDescription { get; set; }
        public string VIN { get; set; }
        public string Series { get;  set; }
        public string Guide { get;  set; }
        public string ShopEquipment { get;  set; }
        public string ExteriorColour { get;  set; }
        public string Options { get;  set; }
        public List<MvsDataViewModel> Models { get; internal set; }

        //public string Marque
        //{
        //    get { return Mvs[..3]; }
        //}
        //public string Model
        //{
        //    get { return Mvs.Substring(3, 3); }
        //}
        //public string Version
        //{
        //    get { return Mvs.Substring(6, 1); }
        //}
        public string Caratt { get; set; }

    }
}
