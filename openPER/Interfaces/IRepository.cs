using openPER.Models;
using System.Collections.Generic;

namespace openPER.Interfaces
{
    public interface IRepository
    {
        TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode);
        List<MakeViewModel> GetAllMakes();
        List<ModelViewModel> GetAllModels(string make);
        List<ModelViewModel> GetAllModels();
        List<CatalogueViewModel> GetAllCatalogues(string make, string model, string languageCode);
        List<GroupViewModel> GetGroupsForCatalogue(string catalogueCode, string languageCode);
        MvsViewModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries);
    }
}
