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

        public List<CatalogueModel> GetAllCatalogues(string make, string model, string languageCode)
        {
            var cacheKeys = new { type = "GetAllCatalogues", k1 = make, k2 = model, k3 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<CatalogueModel> rc))
            {
                rc = _rep.GetAllCatalogues(make, model, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<LanguageModel> GetAllLanguages()
        {
            var cacheKeys = new { type = "GetAllLanguages"};
            if (!_cache.TryGetValue(cacheKeys, out List<LanguageModel> rc))
            {
                rc = _rep.GetAllLanguages();
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

        public List<ModelModel> GetAllModelsForMake(string make)
        {
            var cacheKeys = new { type = "GetAllModels", k1 = make };
            if (!_cache.TryGetValue(cacheKeys, out List<ModelModel> rc))
            {
                rc = _rep.GetAllModelsForMake(make);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<ModelModel> GetAllModels()
        {
            var cacheKeys = new { type = "GetAllModels"};
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
            var cacheKeys = new { type = "GetGroupsForCatalogue", k1 = catalogueCode, k2=languageCode };
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
            var cacheKeys = new { type = "GetSubGroupsForCatalogueGroup", k1 = catalogueCode,k2=groupCode, k3 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<SubGroupModel> rc))
            {
                rc = _rep.GetSubGroupsForCatalogueGroup(catalogueCode,groupCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            var cacheKeys = new { type = "GetMvsDetails", k1 = mvsCode, k2 = mvsVersion, k3 = mvsSeries, k4 = colourCode, k5=languageCode };
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
            var cacheKeys = new { type = "GetPartDetails", k1 = partNumberSearch, k2 = languageCode};
            if (!_cache.TryGetValue(cacheKeys, out PartModel rc))
            {
                rc = _rep.GetPartDetails(partNumberSearch, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public TableModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode)
        {
            var cacheKeys = new { type = "GetTable", k1 = makeCode, k2 = modelCode, k3=catalogueCode, k4=groupCode, k5=subGroupCode, k6=sgsCode, k7=drawingNumber, k8=languageCode };
            if (!_cache.TryGetValue(cacheKeys, out TableModel rc))
            {
                rc = _rep.GetTable(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, sgsCode, drawingNumber, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            var cacheKeys = new { type = "GetSubSubGroupsForCatalogueGroupSubGroup", k1 = catalogueCode, k2 = groupCode, k3 = languageCode };
            if (!_cache.TryGetValue(cacheKeys, out List<SubSubGroupModel> rc))
            {
                rc = _rep.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode,subGroupCode, languageCode);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
                _cache.Set(cacheKeys, rc, cacheEntryOptions);
            }
            return rc;
        }
    }
}
