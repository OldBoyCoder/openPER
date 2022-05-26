using openPER.Models;

namespace openPER.Interfaces
{
    public interface IRepository
    {
        TableViewModel GetTable(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int sgsCode, string languageCode);
    }
}
