using Microsoft.Extensions.Caching.Memory;
using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace openPER.Repositories
{
    public class VersionedRepository : IVersionedRepository
    {
        private readonly IRepository _repository18;
        private readonly IRepository _repository84;
        public VersionedRepository(IMemoryCache cache, IConfiguration config)
        {
            _repository18 = new CachedRelease18Repository(cache, config);
            _repository84 = new Release84Repository(config);
        }
        public List<CatalogueModel> GetAllCatalogues(int release, string make, string model, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetAllCatalogues(make, model, languageCode),
                84 => _repository84.GetAllCatalogues(make, model, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<LanguageModel> GetAllLanguages(int release)
        {
            return release switch
            {
                18 => _repository18.GetAllLanguages(),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<MakeModel> GetAllMakes(int release)
        {
            return release switch
            {
                18 => _repository18.GetAllMakes(),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<ModelModel> GetAllModels(int release)
        {
            return release switch
            {
                18 => _repository18.GetAllModels(),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<ModelModel> GetAllModelsForMake(int release, string make)
        {
            return release switch
            {
                18 => _repository18.GetAllModelsForMake(make),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<GroupModel> GetGroupsForCatalogue(int release, string catalogueCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetGroupsForCatalogue(catalogueCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public MvsModel GetMvsDetails(int release, string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, colourCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public PartModel GetPartDetails(int release, string partNumberSearch, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetPartDetails(partNumberSearch, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(int release, string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode,
                    languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(int release, string catalogueCode, int groupCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public TableModel GetTable(int release, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetTable(catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(int releaseCode, string makeCode, string modelCode, string catalogueCode,
            int groupCode, int subGroupCode, int subSubGroupCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForSubSubGroup(makeCode,modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode),
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
