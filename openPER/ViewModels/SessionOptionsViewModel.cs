using System.Collections.Generic;
using openPERModels;

namespace openPER.ViewModels
{
    public class SessionOptionsViewModel
    {
        public List<LanguageModel> Languages { get; set; }
        public string CurrentLanguage { get; set; }
        public int CurrentVersion { get; set; }

    }
}
