using System.Collections.Generic;

namespace openPER.Models
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
                var rc = Description;
                foreach (var item in Activations)
                {
                    rc += $" {item.ActivationCode} {item.ActivationDescription} ({item.VariationType}{item.VariationCode} {item.VariationDescription} {item.OptionType}{item.OptionCode}{item.OptionDescription})";
                }
                return rc;
            }
        }

    }
}
