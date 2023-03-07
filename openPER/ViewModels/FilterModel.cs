using System.Collections.Generic;

namespace openPER.ViewModels
{
    public enum FilterDataSource
    {
        SINCOM,
        VIN

    }
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
        public FilterDataSource DataSource { get; set; }
    }
}