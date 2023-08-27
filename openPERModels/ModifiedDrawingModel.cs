using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openPERModels
{
    public class ModifiedDrawingModel
    {
        public int GroupCode { get; set; }
        public int SubGroupCode { get; set; }
        public int SubSubGroupCode { get; set; }
        public int DrawingNumber { get; set; }
        public string GroupDescription { get; set; }
        public string SubGroupDescription { get; set; }
        public string SubSubGroupDescription { get; set; }

    }
}
