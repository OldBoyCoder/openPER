namespace openPER.Models
{
    public class MvsViewModel
    {
        public string MvsCode { get; set; }
        public string MvsVersion { get; set; }
        public string MvsSeries { get; set; }

        public string Description { get; set; }
        public string SincomVersion { get; set; }
        public string EngineType { get; set; }
        public string EngineDescription { get; set; }
        public string EngineCode { get; set; }
        public string VariantDescription { get; set; }
        public string VariantCode { get; set; }
        public string CatalogueCode { get; internal set; }
        public string CatalogueDescription { get; internal set; }
        public string ColourCode { get; internal set; }
        public string ColourDescription { get; internal set; }
        public string FullMvsCode
        {
            get
            {
                return $"{MvsCode}.{MvsVersion}.{MvsSeries}";
            }
        }

        public string ModelDescription { get; internal set; }
    }
}
