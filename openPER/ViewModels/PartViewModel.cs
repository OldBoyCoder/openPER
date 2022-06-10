using openPERModels;

namespace openPER.ViewModels
{
    public class PartViewModel
    {
        public string PartNumberSearch { get; set; }
        public PartModel Result { get; set; }
        public int ReleaseCode { get; set; }
    }
}
