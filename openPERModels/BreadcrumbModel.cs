namespace openPERModels
{
    public class BreadcrumbModel
    {
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int? GroupCode { get; set; }
        public int? SubGroupCode { get; set; }
        public int? SubSubGroupCode { get; set; }
        public int? DrawingNumber { get; set; }
        public double ClichePartNumber { get; set; }
        public int ClicheDrawingNumber { get; set; }

        public string MakeDescription { get; set; }
        public string SubMakeDescription { get; set; }
        public string ModelDescription { get; set; }
        public string CatalogueDescription { get; set; }
        public string GroupDescription { get; set; }
        public string SubGroupDescription { get; set; }
        public string SubSubGroupDescription { get; set; }


    }
}
