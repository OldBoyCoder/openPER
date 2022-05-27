﻿using System.Collections.Generic;

namespace openPER.Models
{
    public class GroupViewModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<SubGroupViewModel> SubGroups { get; set; }
    }
}
