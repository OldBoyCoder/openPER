using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable StringLiteralTypo

namespace openPER.Repositories
{
    public class Release84Repository : IRepository
    {
        IConfiguration _config;
        private readonly string _pathToDb;
        public Release84Repository(IConfiguration config)
        {
            _config = config;
            var s = config.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s.FirstOrDefault(x => x.Release == 84);
            if (release != null)
            {
                _pathToDb = System.IO.Path.Combine(release.FolderName, release.DbName);
            }


        }

        public TableModel GetTable(string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber,
            string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<MakeModel> GetAllMakes()
        {
            throw new System.NotImplementedException();
        }

        public List<ModelModel> GetAllModelsForMake(string make, string subMake)
        {
            throw new System.NotImplementedException();
        }

        public List<ModelModel> GetAllModels()
        {
            throw new System.NotImplementedException();
        }

        public List<CatalogueModel> GetAllCatalogues(string make, string subMake, string model, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<GroupModel> GetGroupsForCatalogue(string catalogueCode, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(string catalogueCode, int groupCode, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(string catalogueCode, int groupCode, int subGroupCode,
            string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public PartModel GetPartDetails(string partNumberSearch, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public MvsModel GetMvsDetails(string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<LanguageModel> GetAllLanguages()
        {
            throw new System.NotImplementedException();
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode)
        {
            throw new System.NotImplementedException();
        }

        public List<DrawingKeyModel> GetDrawingKeysForCatalogue(string makeCode, string modelCode, string catalogueCode)
        {
            throw new System.NotImplementedException();
        }

        public List<DrawingKeyModel> GetDrawingKeysForGroup(string makeCode, string modelCode, string catalogueCode, int groupCode)
        {
            throw new System.NotImplementedException();
        }

        public List<DrawingKeyModel> GetDrawingKeysForSubGroup(string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode)
        {
            throw new System.NotImplementedException();
        }

        public string GetMapForCatalogue(string makeCode, string modelCode, string catalogueCode, string mapName)
        {
            throw new System.NotImplementedException();
        }

        public void PopulateBreadcrumbDescriptions(BreadcrumbModel breadcrumb, string languageCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
