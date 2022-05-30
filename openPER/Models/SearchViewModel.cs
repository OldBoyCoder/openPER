namespace openPER.Models
{
    public class SearchViewModel
    {
        public VinSearchViewModel VinSearch { get; set; }
        public string PartNumber { get; set; }
        public string FullVin { get; set; }
        public SearchViewModel()
        {
            VinSearch = new VinSearchViewModel();
        }
    }
}
