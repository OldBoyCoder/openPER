using Microsoft.Extensions.Configuration;
using openPERRepositories.Interfaces;

namespace openPERRepositories.Repositories
{
    public class VersionedImageRepository : IVersionedImageRepository
    {
        readonly IImageRepository _repository18;
        IImageRepository _repository84;
        public VersionedImageRepository(IConfiguration config)
        {
            _repository18 = new Release18ImageRepository(config);
        }
        public byte[] GetImageForModel(int releaseCode, string makeCode,string subMake, string cmgCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForModel(makeCode,subMake, cmgCode),
                _ => null
            };
        }

        public byte[] GetImageForCatalogue(int releaseCode, string makeCode, string subMakeCode, string modelCode, string catalogueCode, string mapName)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode, mapName),
                _ => null
            };
        }

        public byte[] GetImageForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber),
                _ => null
            };
        }

        public byte[] GetThumbnailForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            return releaseCode switch
            {
                18 => _repository18.GetThumbnailForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber),
                _ => null
            };
        }
    }
}
