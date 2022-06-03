

namespace openPER.Models
{
    /// <summary>
    /// Uniquely identifies a drawing in the system
    /// </summary>
    public class DrawingKeyModel
    {
        public int ReleaseCode { get; set; }
        public string MakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; set; }
        public int SubGroupCode { get; set; }
        public int SubSubGroupCode { get; set; }
        public int DrawingNumber { get; set; }
    }
}
