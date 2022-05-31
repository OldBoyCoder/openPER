using openPER.Models;
using System.Collections.Generic;

namespace openPER.Interfaces
{
    public interface IRepository
    {
        TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode);
        List<MakeModel> GetAllMakes();
        List<ModelModel> GetAllModelsForMake(string make);
        List<ModelModel> GetAllModels();
        List<CatalogueModel> GetAllCatalogues(string make, string model, string languageCode);
        List<GroupViewModel> GetGroupsForCatalogue(string catalogueCode, string languageCode);
        PartModel GetPartDetails(string partNumberSearch, string languageCode);
        MvsViewModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode);
        List<LanguageModel> GetAllLanguages();
    }

}
