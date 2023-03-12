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
        public SearchViewModel()
        {
            VinSearch = new VinSearchViewModel();
        }
    }
}
