using openPERRepositories.Interfaces;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openPERPrinting
{
    public class PdfPrint
    {
        IRepository _rep;
        public PdfPrint(IRepository rep)
        {
            _rep = rep;
        }

        public Stream CreateDocument()
        {
            var doc = new PdfDocument();
            var p = doc.AddPage();
            var ms = new MemoryStream();
            doc.Save(ms, false);
            return ms;
        }
    }
}
