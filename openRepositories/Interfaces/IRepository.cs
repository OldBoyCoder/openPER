using System.Collections.Generic;
using openPERModels;

namespace openPERRepositories.Interfaces
{
    public interface IRepository
    {
        TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode);
        List<MakeModel> GetAllMakes();
        List<ModelModel> GetAllModelsForMake(string make, string subMake);
        List<CatalogueModel> GetAllCatalogues(string make,string subMake, string model, string languageCode);
        List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode);
        List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode);
        PartModel GetPartDetails(string partNumberSearch, string languageCode);
        List<PartModel> GetPartSearch(string modelName, string partDescription, string languageCode);
        List<VinSearchResultModel> FindMatchesForVin(string language, string fullVin);
        List<LanguageModel> GetAllLanguages();
        List<DrawingKeyModel> GetDrawingKeysForSubSubGroup( string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode);
        List<MvsDataModel> GetMvsDetails(string mvs);
        List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode);
        List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string languageCode);
        List<VmkModel> GetVmkDataForCatalogue(string catalogueCode, string language);
        string GetVehiclePattern(string language, string vIn);
        MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode);
        string GetImageNameForModel(string makeCode, string subMakeCode, string modelCode);
        void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode);
        List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode);
        Dictionary<string, string> GetFiltersForVehicle(string language,string vIn, string mVs);
        MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group);
        List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode);
        List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string clichePartNumber);
        List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber, int clicheDrawingNumber,
            string languageCode);

        List<ModelModel> GetAllVinModels();
        string GetInteriorColourDescription(string catCode, string interiorColourCode, string language);
        string GetSincomPattern(string mVs);
        string GetExteriorColourDescription(string catCode, string exteriorColourCode, string language);
        public List<MvsCatalogueOptionModel> GetMvsDetailsForCatalogue(string catalogueCode, string language);
        public List<VinSearchResultModel> FindMatchesForMvsAndVin(string language, string mvs, string fullVin);
        public List<CatalogueVariantsModel> GetCatalogueVariants(string catalogueCode);
        public List<ModificationModel> GetCatalogueModifications(string catalogueCode, string language);
        public List<PartModel> GetBasicPartSearch(string modelName, string partDescription, string languageCode);

        public string GetGroupDescription(int groupCode, string languageCode);
        public string GetSubGroupDescription(int groupCode,
            int subSubGroupCode, string languageCode);
        public string GetSubSubGroupDescription(string catalogCode, int groupCode, int subGroupCode,
            int subSubGroupCode, string languageCode);

        public List<MakeModel> GetCatalogueHierarchy(string languageCode);
        public List<GroupModel> GetAllSectionsForCatalogue(string languageCode, string catalogueCode);

    }

}
