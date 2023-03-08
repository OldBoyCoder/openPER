using System;
using System.Collections.Generic;
namespace openPER.ViewModels
{
    public class MvsDataViewModel
    {
        public string MvsMark { get; set; }
        public string MvsModel { get; set; }
        public string MvsVersion { get; set; }
        public string MvsSeries { get; set; }
        public string MvsGuide { get; set; }
        public string MvsShopEquipment { get; set; }

        public string Description { get; set; }
        public string Sincom { get; set; }
        public string Pattern { get; set; }
        public string EngineType { get; set; }
        public string EngineDescription { get; set; }
        public string EngineCode { get; set; }
        public string VariantDescription { get; set; }
        public string VariantCode { get; set; }
        public string CatalogueCode { get; set; }
        public string CatalogueDescription { get; set; }
        public string ColourCode { get; set; }
        public string ColourDescription { get; set; }
        public string FullMvsCode => $"{MvsModel}.{MvsVersion}.{MvsSeries}.{MvsGuide}";

        public string ModelCode { get; set; }
        public string ModelDescription { get; set; }
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public List<Tuple<string, string, string, string>> Options { get; set; }
        public string Language { get; internal set; }
        public FilterModel FilterOptions { get; internal set; }
    }
}
