namespace Arkivverket.Arkade.Core.Util
{
    public interface IChecksumGenerator
    {
        string GenerateChecksum(string pathToFile);
    }
}
