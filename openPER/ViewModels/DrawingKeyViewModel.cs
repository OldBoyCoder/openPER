﻿using System.Security.AccessControl;

namespace openPER.ViewModels
{
    public class DrawingKeyViewModel
    {
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; set; }
        public int SubGroupCode { get; set; }
        public int SubSubGroupCode { get; set; }
        public int Variant { get; set; }
        public int Revision { get; set; }
        public string ClichePartNumber { get; set; }
        public int ClichePartDrawingNumber { get; set; }
        public int ClichePartCode { get; set; }
        public string VariantPattern { get; set; }

        public string FullKey => $"{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{Variant}/{Revision}/{ClichePartNumber}";
    }
}
