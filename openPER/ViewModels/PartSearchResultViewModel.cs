using System.Collections.Generic;
using openPERModels;

namespace openPER.ViewModels;

public class PartSearchResultViewModel
{
    public string PartNumber { get; set; }
    public string Description { get; set; }
    public string FamilyCode { get; set; }
    public string FamilyDescription { get; set; }
    public string UnitOfSale { get; set; }
    public int Weight { get; set; }
    public string CatalogueDescription { get; set; }
    public string CatalogueCode { get; set; }
    public List<PartDrawing> Drawings { get; set; }


}