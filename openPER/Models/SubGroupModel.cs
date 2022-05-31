using System.Collections.Generic;

namespace openPER.Models
{
    public class SubGroupModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<SgsViewModel> SgsGroups { get; set; }
    }
}
