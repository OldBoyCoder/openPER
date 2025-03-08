using openPERModels;
using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SearchViewModel
    {
        public VinSearchViewModel VinSearch { get; set; }
        public string PartNumber { get; set; }
        public string PartModelName { get; set; }
        public string PartName { get; set; }
        public string FullVin { get; set; }
        public string SelectedModel { get; set; }
        public string ChassisNumber { get; set; }
        public string Language { get; set; }
        public string CatSearchPartName { get; set; }
        public List<string> CatalogueCodes { get; set; }
        public List<MakeModel> AllLinks { get; internal set; }
        public string CurrentCatalogue { get; set; }

        public SearchViewModel()
        {
            VinSearch = new VinSearchViewModel();
        }
    }
}
