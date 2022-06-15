using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
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
        public List<CatalogueModel> GetAllCatalogues(int release, string make, string subMake, string model, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetAllCatalogues(make,subMake, model, languageCode),
                84 => _repository84.GetAllCatalogues(make,subMake, model, languageCode),
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
                84 => _repository84.GetAllMakes(),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<ModelModel> GetAllModels(int release)
        {
            return release switch
            {
                18 => _repository18.GetAllModels(),
                84 => _repository84.GetAllModels(),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<ModelModel> GetAllModelsForMake(int release, string make, string subMake)
        {
            return release switch
            {
                18 => _repository18.GetAllModelsForMake(make, subMake),
                84 => _repository84.GetAllModelsForMake(make, subMake),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<GroupModel> GetGroupsForCatalogue(int release, string catalogueCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetGroupsForCatalogue(catalogueCode, languageCode),
                84 => _repository84.GetGroupsForCatalogue(catalogueCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public MvsModel GetMvsDetails(int release, string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, colourCode, languageCode),
                84 => _repository84.GetMvsDetails(mvsCode, mvsVersion, mvsSeries, colourCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public PartModel GetPartDetails(int release, string partNumberSearch, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetPartDetails(partNumberSearch, languageCode),
                84 => _repository84.GetPartDetails(partNumberSearch, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(int release, string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode,
                    languageCode),
                84 => _repository84.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode, subGroupCode,
                    languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(int release, string catalogueCode, int groupCode, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode),
                84 => _repository84.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public TableModel GetTable(int release, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string languageCode)
        {
            return release switch
            {
                18 => _repository18.GetTable(catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, languageCode),
                84 => _repository84.GetTable(catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(int releaseCode, string makeCode, string modelCode, string catalogueCode,
            int groupCode, int subGroupCode, int subSubGroupCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode),
                84 => _repository84.GetDrawingKeysForSubSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(int releaseCode, string makeCode, string modelCode, string catalogueCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForCatalogue(makeCode, modelCode, catalogueCode),
                84 => _repository84.GetDrawingKeysForCatalogue(makeCode, modelCode, catalogueCode),
                _ => throw new System.NotImplementedException()
            };
        }
        public List<DrawingKeyModel> GetDrawingKeysForGroup(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode),
                84 => _repository84.GetDrawingKeysForGroup(makeCode, modelCode, catalogueCode, groupCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode),
                84 => _repository84.GetDrawingKeysForSubGroup(makeCode, modelCode, catalogueCode, groupCode, subGroupCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public MapImageModel GetMapAndImageForCatalogue(int releaseCode, string make, string subMake, string model,
            string catalogue)
        {
            return releaseCode switch
            {
                18 => _repository18.GetMapAndImageForCatalogue(make, subMake, model, catalogue),
                84 => _repository84.GetMapAndImageForCatalogue(make, subMake, model, catalogue),
                _ => throw new System.NotImplementedException()
            };
        }

        public void PopulateBreadcrumbDescriptions(int releaseCode, BreadcrumbModel breadcrumb, string languageCode)
        {
            switch (releaseCode)
            {
                case 18:
                    _repository18.PopulateBreadcrumbDescriptions(breadcrumb, languageCode);
                    break;
                case 84:
                    _repository84.PopulateBreadcrumbDescriptions(breadcrumb, languageCode);
                    break;
            }
        }

        public List<GroupImageMapEntryModel> GetGroupMapEntriesForCatalogue(int releaseCode, string catalogueCode, string languageCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetGroupMapEntriesForCatalogue(catalogueCode, languageCode),
                84 => _repository84.GetGroupMapEntriesForCatalogue(catalogueCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public MapImageModel GetMapForCatalogueGroup(int releaseCode, string make, string subMake, string model,
            string catalogue,
            int group)
        {
            return releaseCode switch
            {
                18 => _repository18.GetMapForCatalogueGroup(make, subMake, model, catalogue, group),
                84 => _repository84.GetMapForCatalogueGroup(make, subMake, model, catalogue, group),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<SubGroupImageMapEntryModel> GetSubGroupMapEntriesForCatalogueGroup(int releaseCode, string catalogueCode, int groupCode, string languageCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, languageCode),
                84 => _repository84.GetSubGroupMapEntriesForCatalogueGroup(catalogueCode, groupCode, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }

        public string GetImageNameForDrawing(int releaseCode, string make, string model, string catalogue, int group, int subgroup,
            int subSubGroup, int drawing)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageNameForDrawing(make, model, catalogue, group, subgroup, subSubGroup, drawing),
                84 => _repository84.GetImageNameForDrawing(make, model, catalogue, group, subgroup, subSubGroup, drawing),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<DrawingKeyModel> GetDrawingKeysForCliche(int releaseCode, string makeCode,string subMakeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, decimal clichePartNumber)
        {
            return releaseCode switch
            {
                18 => _repository18.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber),
                84 => _repository84.GetDrawingKeysForCliche(makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, clichePartNumber),
                _ => throw new System.NotImplementedException()
            };
        }

        public string GetImageNameForClicheDrawing(int releaseCode, decimal clichePartNumber, int clichePartDrawingNumber)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageNameForClicheDrawing(clichePartNumber, clichePartDrawingNumber),
                84 => _repository84.GetImageNameForClicheDrawing(clichePartNumber, clichePartDrawingNumber),
                _ => throw new System.NotImplementedException()
            };
        }

        public List<TablePartModel> GetPartsForCliche(int releaseCode, string catalogueCode, decimal clichePartNumber,
            int clicheDrawingNumber, string languageCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, languageCode),
                84 => _repository84.GetPartsForCliche(catalogueCode, clichePartNumber, clicheDrawingNumber, languageCode),
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
