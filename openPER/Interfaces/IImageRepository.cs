namespace openPER.Interfaces
{
    public interface IImageRepository
    {
        byte[] GetImageForCatalogue(string makeCode, string cmgCode);
    }
}
