using System.Collections.Generic;
using System.Linq;

namespace openPER.ViewModels
{
    public class ModificationViewModel
    {
        public string Type { get; set; }
        public int Code { get; set; }
        public string Sequence { get; set; }
        public string Description { get; set; }
        public int Progression { get; set; }

        public List<ActivationViewModel> Activations { get; set; }
        public string FullDescription
        {
            get
            {
                var rc = $"{Code} - {Description}";
                if (Activations.Count > 0) rc += " (";
                rc += string.Join(',', Activations.Select(x => $"{x.ActivationCode.Trim()} {x.ActivationDescription.Trim()}"));
                if (Activations.Count > 0) rc += ")";
                return rc;
            }
        }

    }
}
