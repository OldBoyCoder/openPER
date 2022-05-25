using System.Collections.Generic;

namespace openPER.Models
{
    public class TableViewModel
    {
        public string MakeCode { get; set; }    
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public string GroupCode { get; set; }
        public string SubGroupCode { get; set; }
        public string SgsCode { get; set; }
        public string ModelDesc { get; set; }
        public string MakeDesc { get; set; }
        public string CatalogueDesc { get; set; }
        public string GroupDesc { get; set; }
        public string SubGroupDesc { get; set; }
        public string SgsDesc { get; set; }
        public List<TablePartViewModel> Parts { get; set; }
        public List<TablePartViewModel> Cliches { get; set; }
        public List<TablePartViewModel> Kits { get; set; }

        public  string Path
        {
            get { return $"{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode:00}/{SubGroupCode:000}/{SgsCode:00}"; }

        }

    }
}
