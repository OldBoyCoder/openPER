using System.Collections.Generic;

namespace openPERModels
{
    public class GroupModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<SubGroupModel> SubGroups { get; set; }
    }
}
