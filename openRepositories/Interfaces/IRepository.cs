using System.Collections.Generic;
using openPERModels;

namespace openPERRepositories.Interfaces
{
    public interface IRepository
    {
        TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode);
        List<MakeModel> GetAllMakes();
        List<ModelModel> GetAllModelsForMake(string make, string subMake);
        List<ModelModel> GetAllModels();
        List<CatalogueModel> GetAllCatalogues(string make,string subMake, string model, string languageCode);
        List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode);
        List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode);
        PartModel GetPartDetails(string partNumberSearch, string languageCode);
        MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode);
        List<LanguageModel> GetAllLanguages();
        List<DrawingKeyModel> GetDrawingKeysForSubSubGroup( string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode);

        List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode);
        List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode);
        List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode);
        MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode);
        void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode);
        List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode);
        MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group);
        List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        string GetImageNameForDrawing(string make, string model, string catalogue, int group, int subgroup, int subSubGroup, int drawing);
        List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, decimal clichePartNumber);
        string GetImageNameForClicheDrawing(decimal clichePartNumber, int clichePartDrawingNumber);
        List<TablePartModel> GetPartsForCliche(string catalogueCode, decimal clichePartNumber, int clicheDrawingNumber,
            string languageCode);

        List<ModelModel> GetAllVinModels();
    }

}
