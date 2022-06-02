namespace openPER.Interfaces
{
    public interface IImageRepository
    {
        byte[] GetImageForCatalogue(string cmgCode);
    }
}
