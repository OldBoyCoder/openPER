using System.Collections.Generic;
using openPERModels;

namespace openPERRepositories.Interfaces
{
    public interface IRepository
    {
        TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode);
        List<MakeModel> GetAllMakes();
        List<ModelModel> GetAllModelsForMake(string make, string subMake);
        List<ModelModel> GetAllModels();
        List<CatalogueModel> GetAllCatalogues(string make,string subMake, string model, string languageCode);
        List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode);
        List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode);
        PartModel GetPartDetails(string partNumberSearch, string languageCode);
        List<PartModel> GetPartSearch(string modelName, string partDescription, string languageCode);
        List<MvsData> GetMvsDetails(string mvsMarque, string mvsModel, string mvsVersion, string mvsSeries, string mvsGuide, string mvsShopEquipment,string colourCode, string languageCode);
        List<LanguageModel> GetAllLanguages();
        List<DrawingKeyModel> GetDrawingKeysForSubSubGroup( string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode);

        List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode, string languageCode);
        List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode);
        List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string languageCode);
        MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode);
        string GetImageNameForModel(string makeCode, string subMakeCode, string modelCode);
        void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode);
        List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode);
        MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group);
        List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        string GetImageNameForDrawing( string catalogue, int group, int subgroup, int subSubGroup, int variant, int revision);
        List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string clichePartNumber);
        string GetImageNameForClicheDrawing(string clichePartNumber, int clichePartDrawingNumber);
        List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber, int clicheDrawingNumber,
            string languageCode);

        List<ModelModel> GetAllVinModels();
        string GetInteriorColourDescription(string catCode, string interiorColourCode, string language);
        string GetExteriorColourDescription(string catCode, string exteriorColourCode, string language);
        string GetOptionCodeDescription(string catCode, string code, string language);
        string GetOptionValueDescription(string catCode, string code, string value, string language);
        string GetOptionValueDescription(string catCode, string code, string language);
    }

}
