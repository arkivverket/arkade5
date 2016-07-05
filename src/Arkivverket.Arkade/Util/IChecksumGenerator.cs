namespace Arkivverket.Arkade.Util
{
    public interface IChecksumGenerator
    {
        string GenerateChecksum(string pathToFile);
    }
}
