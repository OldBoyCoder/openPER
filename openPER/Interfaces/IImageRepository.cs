namespace openPER.Interfaces
{
    public interface IImageRepository
    {
        byte[] GetImageForCatalogue(string makeCode,string subMakeCode, string cmgCode);
        byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode, int subSubGroupCode, int drawingNumber);
        byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber);
    }
}
