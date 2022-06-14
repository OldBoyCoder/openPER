using openPERModels;

namespace openPERRepositories.Interfaces
{
    public interface IImageRepository
    {
        byte[] GetImageForModel(string makeCode, string subMakeCode, string cmgCode);
        byte[] GetSmallImageForModel(string makeCode, string subMakeCode, string cmgCode);
        byte[] GetImageForCatalogue(string makeCode, string subMakeCode, string modelCode, string catalogueCode,
            MapImageModel mapDetails);
        byte[] GetImageForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subgroupCode, int subSubGroupCode, int drawingNumber, string imageName);
        byte[] GetThumbnailForDrawing(string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string imageName);
        byte[] GetThumbnailForCliche(string clichePartNumber, int clichePartDrawingNumber, string imageName);
    }
}
