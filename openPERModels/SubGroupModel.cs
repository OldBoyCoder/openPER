﻿using System.Collections.Generic;

namespace openPERModels
{
    public class SubGroupModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public int GroupCode { get; set; }
        public List<SubSubGroupModel> SubSubGroups { get; set; }
    }
}
