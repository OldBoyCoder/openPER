using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace openPER.Repositories
{
    public class VersionedRepository : IVersionedRepository
    {
        readonly IRepository _repository18;
        IRepository _repository84;
        public VersionedRepository(IConfiguration config)
        {
            _repository18 = new Release18Repository(config);
        }
        public List<CatalogueModel> GetAllCatalogues(int release, string make, string model, string languageCode)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetAllCatalogues(make, model, languageCode);
            }
            throw new System.NotImplementedException();
        }

        public List<LanguageModel> GetAllLanguages(int release)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetAllLanguages();
            }
            throw new System.NotImplementedException();
        }

        public List<MakeModel> GetAllMakes(int release)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetAllMakes();
            }
            throw new System.NotImplementedException();
        }

        public List<ModelModel> GetAllModels(int release)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetAllModels();
            }
            throw new System.NotImplementedException();
        }

        public List<ModelModel> GetAllModelsForMake(int release, string make)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetAllModelsForMake(make);
            }
            throw new System.NotImplementedException();
        }

        public List<GroupModel> GetGroupsForCatalogue(int release, string catalogueCode, string languageCode)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetGroupsForCatalogue(catalogueCode, languageCode);
            }
            throw new System.NotImplementedException();
        }

        public MvsModel GetMvsDetails(int release, string mvsCode, string mvsVersion, string mvsSeries, string colourCode, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public PartModel GetPartDetails(int release, string partNumberSearch, string languageCode)
        {
            throw new System.NotImplementedException();
        }

        public List<SubSubGroupModel> GetSubSubGroupsForCatalogueGroupSubGroup(int release, string catalogueCode, int groupCode, int subGroupCode, string languageCode)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetSubSubGroupsForCatalogueGroupSubGroup(catalogueCode, groupCode,subGroupCode, languageCode);
            }
            throw new System.NotImplementedException();
        }

        public List<SubGroupModel> GetSubGroupsForCatalogueGroup(int release, string catalogueCode, int groupCode, string languageCode)
        {
            switch (release)
            {
                case 18:
                    return _repository18.GetSubGroupsForCatalogueGroup(catalogueCode,groupCode, languageCode);
            }
            throw new System.NotImplementedException();
        }

        public TableModel GetTable(int release, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, int drawingNumber, string languageCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
