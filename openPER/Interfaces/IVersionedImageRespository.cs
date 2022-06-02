namespace openPER.Interfaces
{
    public interface IVersionedImageRespository
    {
        byte[] GetImageForCatalogue(int releaseCode, string makeCode, string cmgCode);
    }
}
