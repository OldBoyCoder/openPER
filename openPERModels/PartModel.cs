﻿using System.Collections.Generic;

namespace openPERModels
{
    public class PartModel
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string FamilyCode { get; set; }
        public string FamilyDescription { get; set; }
        public string UnitOfSale { get; set; }
        public int Weight { get; set; }
        public List<PartDrawing> Drawings { get; set; }
        public List<PartReplacementModel> Replaces { get; set; }
        public List<PartReplacementModel> ReplacedBy { get; set; }
        public string CatalogueDescription { get; set; }
        public string CatalogueCode { get; set; }
        public bool Refurbished { get; set; }
        public bool Accessory { get; set; }
        public bool Orderable { get; set; }
        public bool Replaced { get; set; }
        public bool Exhausted { get; set; }
        public List<PartPriceModel> Prices { get; set; }
    }
}
