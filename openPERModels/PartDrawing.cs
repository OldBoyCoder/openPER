namespace openPERModels
{
    public class PartDrawing
    {
        public string Make { get; set; }
        public string SubMake { get; set; }
        public string Model { get; set; }
        public string CatalogueCode { get; set; }
        public string CatalogueDescription { get; set; }
        public int GroupCode { get; set; }
        public int SubGroupCode { get; set; }
        public string SubGroupDescription { get; set; }
        public int SubSubGroupCode { get; set; }
        public int DrawingNumber { get; set; }
        public decimal ClichePartNumber { get; set; }
        public int ClichePartDrawingNumber { get; set; }
        public bool ClichePart { get; set; }
    }
}
