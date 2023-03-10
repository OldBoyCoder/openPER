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
            var rc = _rep.GetAllModelsForMake(make, subMake);
            return rc;
        }

        public List<ModelModel> GetAllVinModels()
        {
            var rc = _rep.GetAllVinModels();
            return rc;
        }

        public List<CatalogueVariantsModel> GetCatalogueVariants(string catalogueCode)
        {
            var rc = _rep.GetCatalogueVariants(catalogueCode);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForCliche(string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string clichePartNumber)
        {
            var rc = _rep.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, string languageCode)
        {
            var rc = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode, languageCode);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var rc = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, languageCode);
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, string languageCode)
        {
            var rc = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, languageCode);
            return rc;
        }

        public string GetExteriorColourDescription(string catCode, string exteriorColourCode, string language)
        {
            var rc = _rep.GetExteriorColourDescription(catCode, exteriorColourCode, language);
            return rc;
        }

        public Dictionary<string, string> GetFiltersforVehicle(string language, string vIn, string mVs)
        {
            var rc = _rep.GetFiltersforVehicle(language, vIn, mVs);
            return rc;
        }

        public List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode, string languageCode)
        {
            var rc = _rep.GetGroupMapEntriesForCatalogue(catalogueCode, languageCode);
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var rc = _rep.GetGroupsForCatalogue(catalogueCode, languageCode);
            return rc;
        }

        public string GetImageNameForModel(string makeCode, string subMakeCode, string modelCode)
        {
            var rc = _rep.GetImageNameForModel(makeCode, subMakeCode, modelCode);
            return rc;
        }

        public string GetInteriorColourDescription(string catCode, string interiorColourCode, string language)
        {
            var rc = _rep.GetInteriorColourDescription(catCode, interiorColourCode, language);
            return rc;
        }

        public MapImageModel GetMapAndImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode)
        {
            var rc = _rep.GetMapAndImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode);
            return rc;
        }

        public MapImageModel GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
        {
            var rc = _rep.GetMapForCatalogueGroup(make, subMake, model, catalogue, group);
            return rc;
        }

        public List<MvsDataModel> GetMvsDetails(string mvs)
        {
            var rc = _rep.GetMvsDetails(mvs);
            return rc;
        }

        public List<MvsCatalogueOptionModel> GetMvsDetailsForCatalogue(string catalogueCode, string language)
        {
            var rc = _rep.GetMvsDetailsForCatalogue(catalogueCode, language);
            return rc;
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            var rc = _rep.GetPartDetails(partNumberSearch, languageCode);
            return rc;
        }

        public List<PartModel> GetPartSearch(string modelName, string partDescription, string languageCode)
        {
            var rc = _rep.GetPartSearch(modelName, partDescription, languageCode);
            return rc;
        }

        public List<TablePartModel> GetPartsForCliche(string catalogueCode, string clichePartNumber, int clicheDrawingNumber, string languageCode)
        {
            var rc = _rep.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, languageCode);
            return rc;
        }

        public string GetSincomPattern(string mVs)
        {
            var rc = _rep.GetSincomPattern(mVs);
            return rc;
        }

        public List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var rc = _rep.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, languageCode);
            return rc;
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var rc = _rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode);
            return rc;
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var rc = _rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode, languageCode);
            return rc;
        }

        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, int revision, string languageCode)
        {
            var rc = _rep.GetTable(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, revision, languageCode);
            return rc;
        }

        public string GetVehiclePattern(string language, string vIn)
        {
            var rc = _rep.GetVehiclePattern(language, vIn);
            return rc;
        }

        public List<VmkModel> GetVmkDataForCatalogue(string catalogueCode, string language)
        {
            var rc = _rep.GetVmkDataForCatalogue(catalogueCode, language);
            return rc;
        }

        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            _rep.PopulateBreadcrumbDescriptions(breadcrumb, languageCode);
        }
    }
}
