using System.Collections.Generic;

namespace openPER.ViewModels;

public class PartSearchResultsViewModel
{
    public string Language { get; set; }
    public List<PartSearchResultViewModel> Results { get; set; } = new();
    public NavigationViewModel Navigation { get; set; }
}