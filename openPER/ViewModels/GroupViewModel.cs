using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class GroupViewModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string DisplayCode
        {
            get
            {
                return Code.ToString("000");
            }
        }

    }
}
