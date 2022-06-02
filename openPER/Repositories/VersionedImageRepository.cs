using Microsoft.Extensions.Configuration;
using openPER.Interfaces;
namespace openPER.Repositories
{
    public class VersionedImageRepository : IVersionedImageRespository
    {
        IImageRepository _repository18;
        IImageRepository _repository84;
        public VersionedImageRepository(IConfiguration config)
        {
            _repository18 = new Release18ImageRepository(config);
        }
        public byte[] GetImageForCatalogue(int releaseCode, string cmgCode)
        {
            switch (releaseCode)
            {
                case 18:
                    return _repository18.GetImageForCatalogue(cmgCode);
                default:
                    break;
            }
            return null;
        }
    }
}
