namespace openPER.Interfaces
{
    public interface IImageRepository
    {
        byte[] GetImageForModel(string makeCode, string subMakeCode, string cmgCode);
        byte[] GetImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode, string mapName);
        byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode, int subSubGroupCode, int drawingNumber);
        byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber);
    }
}
