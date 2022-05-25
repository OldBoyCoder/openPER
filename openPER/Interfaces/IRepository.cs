using openPER.Models;

namespace openPER.Interfaces
{
    public interface IRepository
    {
        TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, string groupCode, string subGroupCode, string languageCode);
    }
}
