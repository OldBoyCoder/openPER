namespace openPERModels
{
    public class MvsModel
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
        public string CatalogueCode { get;  set; }
        public string CatalogueDescription { get;  set; }
        public string ColourCode { get;  set; }
        public string ColourDescription { get;  set; }
        public string FullMvsCode => $"{MvsCode}.{MvsVersion}.{MvsSeries}";

        public string ModelDescription { get;  set; }
    }
}
