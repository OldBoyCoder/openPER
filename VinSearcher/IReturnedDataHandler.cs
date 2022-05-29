using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinSearcher
{
    public interface IReturnedDataHandler
    {
        bool ProcessRow(List<string> data, string searchKey);

    }
}
