﻿using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class ModelsViewModel:VersionedViewModel
    {
        public List<ModelViewModel> Models{ get; set; }
        public string MakeCode { get; set; }
        public NavigationViewModel Navigation { get; set; }

    }
}

