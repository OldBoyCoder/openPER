

namespace openPERModels
{
    /// <summary>
    /// Uniquely identifies a drawing in the system
    /// </summary>
    public class DrawingKeyModel
    {
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; set; }
        public int SubGroupCode { get; set; }
        public int SubSubGroupCode { get; set; }
        public int Revision { get; set; }
        public string ClichePartNumber { get; set; }
        public int ClichePartDrawingNumber { get; set; }
        public string ClichePartCode { get; set; }
        public string VariantPattern { get; set; }
        public int Variant { get; set; }
        public string RevisionModifications { get; set; }
        public string Description { get; set; }

    }
}
