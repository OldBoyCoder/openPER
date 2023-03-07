using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class FilterModel
    {
        public string VIN { get; set; }
        public string MVS { get; set; }
        //aka ricambio
        public string NumberForParts { get; set; }
        public string Engine { get; set; }
        public string BuildDate { get; set; }
        public string Pattern { get; set; }
        public List<FilterOptions> Options { get; set; } = new List<FilterOptions>();
    }
    public class FilterOptions
    {
        public string TypeDescription { get; set; }
        public string TypeCode { get; set; }
        public bool MultiValue { get; set; }
        public string ValueCode { get; set; }
        public string ValueDescription { get; set; }
    }
}