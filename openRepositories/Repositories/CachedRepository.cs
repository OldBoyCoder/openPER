using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;
using System;
using System.Collections.Generic;

namespace openPERRepositories.Repositories
{
    public class CachedRepository : IRepository
    {
        readonly IRepository _rep;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _options;

        public CachedRepository(IConfiguration config, IMemoryCache cache)
        {
            _cache = cache;
            _rep = new Release84Repository(config);
            _options = new MemoryCacheEntryOptions();
            _options.SetSlidingExpiration(TimeSpan.FromSeconds(300));

        }
        public List<VinSearchResultModel> FindMatchesForMvsAndVin(string language, string mvs, string fullVin)
        {
            if (_cache.TryGetValue(("FindMatchesForMvsAndVin", language, mvs, fullVin), out List<VinSearchResultModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.FindMatchesForMvsAndVin(language, mvs, fullVin);
            _cache.Set(("FindMatchesForMvsAndVin", language, mvs, fullVin), rc, _options);
            return rc;
        }

        public List<VinSearchResultModel> FindMatchesForVin(string language, string fullVin)
        {
            if (_cache.TryGetValue(("FindMatchesForVin", language, fullVin), out List<VinSearchResultModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.FindMatchesForVin(language, fullVin);
            _cache.Set(("FindMatchesForVin", language, fullVin), rc, _options);
            return rc;
        }

        public List<CatalogueModel> GetAllCatalogues(string make, string subMake, string model, string languageCode)
        {
            var rc = _rep.GetAllCatalogues(make, subMake, model, languageCode);
            return rc;
        }

        public List<LanguageModel> GetAllLanguages()
        {
            if (_cache.TryGetValue(("GetAllLanguages"), out List<LanguageModel> cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetAllLanguages();
            _cache.Set(("GetAllLanguages"), rc, _options);
            return rc;
        }

        public List<MakeModel> GetAllMakes()
        {
            if (_cache.TryGetValue(("GetAllMakes"), out List<MakeModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetAllMakes();
            _cache.Set(("GetAllMakes"), rc, _options);
            return rc;
        }

        public List<ModelModel> GetAllModelsForMake(string make, string subMake)
        {
            var key = ("GetAllModelsForMake", make, subMake);
            if (_cache.TryGetValue(key, out List<ModelModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetAllModelsForMake(make, subMake);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<ModelModel> GetAllVinModels()
        {
            var key = ("GetAllVinModels");
            if (_cache.TryGetValue(key, out List<ModelModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetAllVinModels();
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<CatalogueVariantsModel> GetCatalogueVariants(string catalogueCode)
        {
            var key = ("GetCatalogueVariants", catalogueCode);
            if (_cache.TryGetValue(key, out List<CatalogueVariantsModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetCatalogueVariants(catalogueCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<ModificationModel> GetCatalogueModifications(string catalogueCode, string language)
        {
            var key = ("GetCatalogueModifications", catalogueCode, language);
            if (_cache.TryGetValue(key, out List<ModificationModel> cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetCatalogueModifications(catalogueCode, language);
            _cache.Set(key, rc, _options);
            return rc;

        }

        public List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string clichePartNumber)
        {
            var key = ("GetDrawingKeysForCliche", makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber);
            if (_cache.TryGetValue(key, out List<DrawingKeyModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode)
        {
            var key = ("GetDrawingKeysForGroup", makeCode, modelCode, catalogueCode, groupCode, languageCode);
            if (_cache.TryGetValue(key, out List<DrawingKeyModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var key = ("GetDrawingKeysForSubGroup", makeCode, modelCode, catalogueCode, groupCode, subGroupCode, languageCode);
            if (_cache.TryGetValue(key, out List<DrawingKeyModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode)
        {
            var key = ("GetDrawingKeysForSubSubGroup", makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            if (_cache.TryGetValue(key, out List<DrawingKeyModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetExteriorColourDescription(string catCode, string exteriorColourCode, string language)
        {
            var key = ("GetExteriorColourDescription", catCode, exteriorColourCode, language);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetExteriorColourDescription(catCode, exteriorColourCode, language);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public Dictionary<string, string> GetFiltersForVehicle(string language, string vIn, string mVs)
        {
            var key = ("GetFiltersForVehicle", language, vIn, mVs);
            if (_cache.TryGetValue(key, out Dictionary<string, string> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetFiltersForVehicle(language, vIn, mVs);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
        {
            var key = ("GetGroupMapEntriesForCatalogue", catalogueCode, languageCode);
            if (_cache.TryGetValue(key, out List<GroupImageMapEntryModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetGroupMapEntriesForCatalogue(catalogueCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var key = ("GetGroupsForCatalogue", catalogueCode, languageCode);
            if (_cache.TryGetValue(key, out List<GroupModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetGroupsForCatalogue(catalogueCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetImageNameForModel(string makeCode, string subMakeCode, string modelCode)
        {
            var key = ("GetImageNameForModel", makeCode, subMakeCode, modelCode);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetImageNameForModel(makeCode, subMakeCode, modelCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetInteriorColourDescription(string catCode, string interiorColourCode, string language)
        {
            var key = ("GetInteriorColourDescription", catCode, interiorColourCode, language);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetInteriorColourDescription(catCode, interiorColourCode, language);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode)
        {
            var key = ("GetMapAndImageForCatalogue", makeCode, subMakeCode, modelCode, catalogueCode);
            if (_cache.TryGetValue(key, out MapImageModel cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetMapAndImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
        {
            var key = ("GetMapForCatalogueGroup", make, subMake, model, catalogue, group);
            if (_cache.TryGetValue(key, out MapImageModel cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetMapForCatalogueGroup(make, subMake, model, catalogue, group);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<MvsDataModel> GetMvsDetails(string mvs)
        {
            var key = ("GetMvsDetails", mvs);
            if (_cache.TryGetValue(key, out List<MvsDataModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetMvsDetails(mvs);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<MvsCatalogueOptionModel> GetMvsDetailsForCatalogue(string catalogueCode, string language)
        {
            var key = ("GetMvsDetailsForCatalogue", catalogueCode, language);
            if (_cache.TryGetValue(key, out List<MvsCatalogueOptionModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetMvsDetailsForCatalogue(catalogueCode, language);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            var key = ("GetPartDetails", partNumberSearch, languageCode);
            if (_cache.TryGetValue(key, out PartModel cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetPartDetails(partNumberSearch, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<PartModel> GetPartSearch(string modelName, string partDescription, string languageCode)
        {
            var key = ("GetPartSearch", modelName, partDescription, languageCode);
            if (_cache.TryGetValue(key, out List<PartModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetPartSearch(modelName, partDescription, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber, int clicheDrawingNumber, string languageCode)
        {
            var key = ("GetPartsForCliche", catalogueCode, clichePartNumber, clicheDrawingNumber, languageCode);
            if (_cache.TryGetValue(key, out List<TablePartModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetSincomPattern(string mVs)
        {
            var key = ("GetSincomPattern", mVs);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetSincomPattern(mVs);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var key = ("GetSubGroupMapEntriesForCatalogueGroup", catalogueCode, groupCode, languageCode);
            if (_cache.TryGetValue(key, out List<SubGroupImageMapEntryModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var key = ("GetSubGroupsForCatalogueGroup", catalogueCode, groupCode, languageCode);
            if (_cache.TryGetValue(key, out List<SubGroupModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var key = ("GetSubSubGroupsForCatalogueGroupSubGroup", catalogueCode, groupCode, subGroupCode, languageCode);
            if (_cache.TryGetValue(key, out List<SubSubGroupModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
        {
            var key = ("GetSubSubGroupsForCatalogueGroupSubGroup", catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision, languageCode);
            if (_cache.TryGetValue(key, out TableModel cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetTable(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetVehiclePattern(string language, string vIn)
        {
            var key = ("GetVehiclePattern", language, vIn);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetVehiclePattern(language, vIn);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<VmkModel> GetVmkDataForCatalogue(string catalogueCode, string language)
        {
            var key = ("GetVmkDataForCatalogue", catalogueCode, catalogueCode, language);
            if (_cache.TryGetValue(key, out List<VmkModel> cacheValue))
            {
                return cacheValue;
            }
            var rc = _rep.GetVmkDataForCatalogue(catalogueCode, language);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, languageCode);
        }

        public List<PartModel> GetBasicPartSearch(string modelName, string partDescription, string languageCode)
        {
            return _rep.GetBasicPartSearch(modelName, partDescription, languageCode);
        }

        public string GetGroupDescription(int groupCode, string languageCode)
        {
            var key = ("GetGroupDescription", groupCode, languageCode);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetGroupDescription(groupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;

        }

        public string GetSubGroupDescription(int groupCode, int subGroupCode, string languageCode)
        {
            var key = ("GetSubGroupDescription", groupCode, subGroupCode, languageCode);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetSubGroupDescription(groupCode, subGroupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public string GetSubSubGroupDescription(string catalogCode, int groupCode, int subGroupCode, int subSubGroupCode,
            string languageCode)
        {
            var key = ("GetSubSubGroupDescription", catalogCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            if (_cache.TryGetValue(key, out string cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetSubSubGroupDescription(catalogCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            _cache.Set(key, rc, _options);
            return rc;
        }

        public List<MakeModel> GetCatalogueHierarchy(string languageCode)
        {
            var key = ("GetCatalogueHierarchy", languageCode);
            if (_cache.TryGetValue(key, out List<MakeModel> cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetCatalogueHierarchy(languageCode);
            _cache.Set(key, rc, _options);
            return rc;

        }

        public List<GroupModel> GetAllSectionsForCatalogue(string languageCode, string catalogueCode)
        {
            var key = ("GetAllSectionsForCatalogue", languageCode, catalogueCode);
            if (_cache.TryGetValue(key, out List<GroupModel> cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetAllSectionsForCatalogue(languageCode, catalogueCode);
            _cache.Set(key, rc, _options);
            return rc;

        }
        public List<ModifiedDrawingModel> GetAllDrawingsForModification(string languageCode, string catalogueCode, int modificationNumber)
        {
            var key = ("GetAllDrawingsForModification", languageCode, catalogueCode, modificationNumber);
            if (_cache.TryGetValue(key, out List<ModifiedDrawingModel> cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetAllDrawingsForModification(languageCode, catalogueCode,modificationNumber);
            _cache.Set(key, rc, _options);
            return rc;

        }
        public ModificationModel GetCatalogueModificationDetail(string catalogueCode, string languageCode, int modification)
        {
            var key = ("GetCatalogueModificationDetail", catalogueCode, languageCode, modification);
            if (_cache.TryGetValue(key, out ModificationModel cacheValue))
            {
                return cacheValue;
            }

            var rc = _rep.GetCatalogueModificationDetail(catalogueCode, languageCode, modification);
            _cache.Set(key, rc, _options);
            return rc;

        }
        public List<PartExportModel> GetAllPartsForCatalogue(string languageCode, string catalogueCode)
        {
            return _rep.GetAllPartsForCatalogue(languageCode, catalogueCode);
        }

    }
}
