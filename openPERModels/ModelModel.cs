using System.Collections.Generic;

namespace openPERModels
{
    public class ModelModel
    {
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public List<CatalogueModel> Catalogues { get; set; }
    }
}
