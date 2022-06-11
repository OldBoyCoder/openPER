using Microsoft.Extensions.Configuration;
using openPERModels;
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
            _repository84 = new Release84ImageRepository(config);
        }
        public byte[] GetImageForModel(int releaseCode, string makeCode, string subMake, string cmgCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForModel(makeCode, subMake, cmgCode),
                84 => _repository84.GetImageForModel(makeCode, subMake, cmgCode),
                _ => null
            };
        }
        public byte[] GetSmallImageForModel(int releaseCode, string makeCode, string subMake, string cmgCode)
        {
            return releaseCode switch
            {
                18 => _repository18.GetSmallImageForModel(makeCode, subMake, cmgCode),
                84 => _repository84.GetSmallImageForModel(makeCode, subMake, cmgCode),
                _ => null
            };
        }

        public byte[] GetImageForCatalogue(int releaseCode, string makeCode, string subMakeCode, string modelCode,
            string catalogueCode, MapImageModel mapDetails)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode, mapDetails),
                84 => _repository84.GetImageForCatalogue(makeCode, subMakeCode, modelCode, catalogueCode, mapDetails),
                _ => null
            };
        }

        public byte[] GetImageForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode,
            int subGroupCode, int subSubGroupCode, int drawingNumber, string imageName)
        {
            return releaseCode switch
            {
                18 => _repository18.GetImageForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, imageName),
                84 => _repository84.GetImageForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, imageName),
                _ => null
            };
        }

        public byte[] GetThumbnailForDrawing(int releaseCode, string makeCode, string modelCode, string catalogueCode,
            int groupCode,
            int subGroupCode, int subSubGroupCode, int drawingNumber, string imageName)
        {
            return releaseCode switch
            {
                18 => _repository18.GetThumbnailForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, imageName),
                84 => _repository84.GetThumbnailForDrawing(makeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, imageName),
                _ => null
            };
        }
    }
}
