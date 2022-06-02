using openPER.Models;
using System.Collections.Generic;

namespace openPER.Interfaces
{
    public interface IVersionedRepository
    {
        List<CatalogueModel> GetAllCatalogues(int release, string make, string model, string languageCode);
        List<LanguageModel> GetAllLanguages(int release);
        List<MakeModel> GetAllMakes(int release);
        List<ModelModel> GetAllModels(int release);
        List<ModelModel> GetAllModelsForMake(int release, string make);
        List<GroupModel> GetGroupsForCatalogue(int release, string catalogueCode, string languageCode);
        MvsModel GetMvsDetails(int release, string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode);
        PartModel GetPartDetails(int release, string partNumberSearch, string languageCode);
        List<SubGroupModel> GetSubGroupsForCatalogueGroup(int release, string catalogueCode, int groupCode, string languageCode);
        TableModel GetTable(int release, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode);
    }
}