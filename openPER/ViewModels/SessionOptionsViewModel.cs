using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using openPERModels;

namespace openPER.ViewModels
{
    public class SessionOptionsViewModel
    {
        public List<LanguageModel> Languages { get; set; }
        public string CurrentLanguage { get; set; }
        public int CurrentVersion { get; set; }
        public RouteValueDictionary RouteData { get; set; }
        public object Action { get; set; }
        public object Controller { get; set; }

    }
}
