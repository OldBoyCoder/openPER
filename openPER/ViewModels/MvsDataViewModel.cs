using System;
using System.Collections.Generic;
namespace openPER.ViewModels
{
    public class MvsDataViewModel
    {
        public string MvsModel { get; set; }
        public string MvsVersion { get; set; }
        public string MvsSeries { get; set; }
        public string MvsGuide { get; set; }

        public string Description { get; set; }
        public string Sincom { get; set; }
        public string EngineType { get; set; }
        public string CatalogueCode { get; set; }
        public string CatalogueDescription { get; set; }
        public string ModelCode { get; set; }
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string Language { get; internal set; }
        public FilterModel FilterOptions { get; internal set; }
    }
}
