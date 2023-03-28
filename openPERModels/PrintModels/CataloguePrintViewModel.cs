using System.Collections.Generic;

namespace openPERModels.PrintModels
{
    public class CataloguePrintViewModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public List<DrawingKeyModel> Drawings { get; set; }
    }
}
