using Microsoft.Extensions.Caching.Memory;
using openPER.Interfaces;
using openPER.Models;
using System;
using System.Collections.Generic;

namespace openPER.Repositories
{
    public class CachedRelease18Repository : IRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IRepository _rep;
        public CachedRelease18Repository(IMemoryCache cache, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _cache = cache;
            _rep = new Release18Repository(config);
        }

        public List<CatalogueModel> GetAllCatalogues(string make, string subMake, string model, string languageCode)
        {
            var cacheKeys = new { type = "GetAllCatalogues", k1 = make, k2 = subMake, k3 = model, k4 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<CatalogueModel> rc))
            {
                rc = _rep.GetAllCatalogues(make, subMake, model, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<LanguageModel> GetAllLanguages()
        {
            var cacheKeys = new { type = "GetAllLanguages" };
            if (!_cache.TryGetValue(cacheKeys, out List<LanguageModel> rc))
            {
                rc = _rep.GetAllLanguages();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode)
        {
            var cacheKeys = new { type = "GetDrawingKeysForSubSubGroup", k1 = makeCode, k2 = modelCode, k3 = catalogueCode, k4 = groupCode, k5 = subGroupCode, k6 = subSubGroupCode };
            if (!_cache.TryGetValue(cacheKeys, out List<DrawingKeyModel> rc))
            {
                rc = _rep.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode)
        {
            var cacheKeys = new { type = "GetDrawingKeysForCatalogue", k1 = makeCode, k2 = modelCode, k3 = catalogueCode };
            if (!_cache.TryGetValue(cacheKeys, out List<DrawingKeyModel> rc))
            {
                rc = _rep.GetDrawingKeysForCatalogue(makeCode, modelCode, catalogueCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode)
        {
            var cacheKeys = new { type = "GetDrawingKeysForGroup", k1 = makeCode, k2 = modelCode, k3 = catalogueCode, k4 = groupCode };
            if (!_cache.TryGetValue(cacheKeys, out List<DrawingKeyModel> rc))
            {
                rc = _rep.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode)
        {
            var cacheKeys = new { type = "GetDrawingKeysForSubGroup", k1 = makeCode, k2 = modelCode, k3 = catalogueCode, k4 = groupCode, k5 = subGroupCode };
            if (!_cache.TryGetValue(cacheKeys, out List<DrawingKeyModel> rc))
            {
                rc = _rep.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public string GetMapForCatalogue(string makeCode,string subMakeCode, string modelCode, string catalogueCode)
        {
            var cacheKeys = new { type = "GetMapForCatalogue", k1 = makeCode,k2=subMakeCode,  k3= modelCode, k4 = catalogueCode};
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                rc = _rep.GetMapForCatalogue(makeCode,subMakeCode, modelCode, catalogueCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            var cacheKeys = new { type = "PopulateBreadcrumbDescriptions", k1 = breadcrumb };
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                _rep.PopulateBreadcrumbDescriptions(breadcrumb, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }

        }

        public List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(string catalogueCode)
        {
            var cacheKeys = new { type = "GetGroupMapEntriesForCatalogue", k1 = catalogueCode };
            if (!_cache.TryGetValue(cacheKeys, out List<GroupImageMapEntryModel> rc))
            {
                rc = _rep.GetGroupMapEntriesForCatalogue(catalogueCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }

            return rc;
        }

        public string GetMapForCatalogueGroup(string make, string subMake, string model, string catalogue, int group)
        {
            var cacheKeys = new { type = "GetMapForCatalogueGroup", k1 = make, k2 = subMake, k3 = model, k4 = catalogue, k5=group };
            if (!_cache.TryGetValue(cacheKeys, out string rc))
            {
                rc = _rep.GetMapForCatalogueGroup(make, subMake, model, catalogue, group);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(string catalogueCode, int groupCode)
        {
            var cacheKeys = new { type = "GetSubGroupMapEntriesForCatalogueGroup", k1 = catalogueCode, k2=groupCode };
            if (!_cache.TryGetValue(cacheKeys, out List<SubGroupImageMapEntryModel> rc))
            {
                rc = _rep.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }

            return rc;
        }

        public List<MakeModel> GetAllMakes()
        {
            var cacheKeys = new { type = "GetAllMakes" };
            if (!_cache.TryGetValue(cacheKeys, out List<MakeModel> rc))
            {
                rc = _rep.GetAllMakes();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<ModelModel> GetAllModelsForMake(string make, string subMake)
        {
            var cacheKeys = new { type = "GetAllModels", k1 = make, k2 = subMake };
            if (!_cache.TryGetValue(cacheKeys, out List<ModelModel> rc))
            {
                rc = _rep.GetAllModelsForMake(make, subMake);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<ModelModel> GetAllModels()
        {
            var cacheKeys = new { type = "GetAllModels" };
            if (!_cache.TryGetValue(cacheKeys, out List<ModelModel> rc))
            {
                rc = _rep.GetAllModels();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            var cacheKeys = new { type = "GetGroupsForCatalogue", k1 = catalogueCode, k2 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<GroupModel> rc))
            {
                rc = _rep.GetGroupsForCatalogue(catalogueCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }
        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            var cacheKeys = new { type = "GetSubGroupsForCatalogueGroup", k1 = catalogueCode, k2 = groupCode, k3 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<SubGroupModel> rc))
            {
                rc = _rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            var cacheKeys = new { type = "GetMvsDetails", k1 = mvsCode, k2 = mvsVersion, k3 = mvsSeries, k4 = colourCode, k5 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out MvsModel rc))
            {
                rc = _rep.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, colourCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            var cacheKeys = new { type = "GetPartDetails", k1 = partNumberSearch, k2 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out PartModel rc))
            {
                rc = _rep.GetPartDetails(partNumberSearch, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode)
        {
            var cacheKeys = new { type = "GetTable", k1 = catalogueCode, k2 = groupCode, k3 = subGroupCode, k4 = sgsCode, k5 = drawingNumber, k6 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out TableModel rc))
            {
                rc = _rep.GetTable(catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var cacheKeys = new { type = "GetSubSubGroupsForCatalogueGroupSubGroup", k1 = catalogueCode, k2 = groupCode, k3 = languageCode };
//            if (!_cache.TryGetValue(cacheKeys, out List<SubSubGroupModel> rc))
//            {
                var rc = _rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
  //          }
            return rc;
        }
    }
}
