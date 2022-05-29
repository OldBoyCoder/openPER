using System.Collections.Generic;
using VinSearcher;

namespace openPER.Models
{
    public class VinSearchViewModel
    {
        public List<ModelViewModel> Models { get; set; }
        public string SelectedModel { get; set; }
        public int ChassisNumber { get; set; }
        public VinResult Result { get; set; }
        public MvsViewModel MvsData { get; set; }
    }
}
