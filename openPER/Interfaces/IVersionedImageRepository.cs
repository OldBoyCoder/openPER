namespace openPER.Interfaces
{
    public interface IVersionedImageRepository
    {
        byte[] GetImageForCatalogue(int releaseCode, string makeCode, string subMakeCode, string cmgCode);
        byte[] GetImageForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode, int subSubGroupCode, int drawingNumber);
        byte[] GetThumbnailForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber);
    }
}
