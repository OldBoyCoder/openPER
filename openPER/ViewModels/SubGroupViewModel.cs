using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubGroupViewModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string DisplayCode
        {
            get
            {
                return Code.ToString("00");
            }
        }
        public string FullDisplayCode
        {
            get
            {
                return GroupCode.ToString("000")+Code.ToString("00");
            }
        }
        public int GroupCode { get; set; }

    }
}
