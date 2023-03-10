using System.Collections.Generic;

namespace openPER.ViewModels
{
    public enum FilterDataSource
    {
        Sincom,
        Vin

    }
    public class FilterModel
    {
        public string Vin { get; set; }
        public string Mvs { get; set; }
        //aka ricambio
        public string NumberForParts { get; set; }
        public string Engine { get; set; }
        public string BuildDate { get; set; }
        public List<FilterOptions> Options { get; set; } = new();
        public FilterDataSource DataSource { get; set; }
    }
}