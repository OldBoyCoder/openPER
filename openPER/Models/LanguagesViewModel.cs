using System.Collections.Generic;
namespace openPER.Models
{
    public class SessionOptionsViewModel
    {
        public List<LanguageModel> Languages { get; set; }
        public string CurrentLanguage { get; set; }

        public List<VersionModel> Versions { get; set; }
        public int CurrentVersion { get; set; }

    }
}
