using ICSharpCode.SharpZipLib.Tar;

namespace Arkivverket.Arkade.Core.Util
{
    public static class TarInputStreamExtension
    {
        public static string GenerateChecksumForEntry(this TarInputStream tarInputStream, IChecksumGenerator checksumGenerator)
        {
            checksumGenerator.Initialize();

            var tempBuffer = new byte[32 * 1024];

            while (true)
            {
                int numRead = tarInputStream.Read(tempBuffer, 0, tempBuffer.Length);
                if (numRead <= 0)
                {
                    break;
                }

                checksumGenerator.TransformBlock(tempBuffer, numRead);
            }
            return checksumGenerator.GenerateChecksum();
        }
    }
}
