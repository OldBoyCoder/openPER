using System.Collections.Generic;

namespace openPERModels
{
    public class CatalogueModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string ImageName { get; set; }
        public List<GroupModel> Groups { get; set; }
    }
}
