using System.Collections.Generic;

namespace openPER.ViewModels;

public class VinSearchResultsViewModel
{
    public List<VinSearchResultViewModel> Results { get; set; }
    public NavigationViewModel Navigation { get; set; }
}