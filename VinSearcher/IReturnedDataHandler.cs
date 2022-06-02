using System.Collections.Generic;

namespace VinSearcher
{
    public interface IReturnedDataHandler
    {
        bool ProcessRow(List<string> data, string searchKey);

    }
}
