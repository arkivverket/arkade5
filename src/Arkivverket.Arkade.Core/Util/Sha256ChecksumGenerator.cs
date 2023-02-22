using System.IO;
using System.Security.Cryptography;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public class Sha256ChecksumGenerator : IChecksumGenerator
    {
        /// <summary>
        ///     Generates a checksum with the SHA-256 algorithm
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public string GenerateChecksum(string pathToFile)
        {
            using FileStream fileStream = new FileInfo(pathToFile).OpenRead();
            
            return GenerateChecksum(fileStream);
        }

        /// <summary>
        /// Generates a checksum with the SHA-256 algorithm
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string GenerateChecksum(Stream stream)
        {
            return Hex.ToHexString(ComputeHash(stream));
        }

        private static byte[] ComputeHash(Stream stream)
        {
            var h = (HashAlgorithm)CryptoConfig.CreateFromName("SHA256");

            if (h == null)
            {
                throw new ArkadeException("Checksum algorithm SHA-256 not supported.");
            }

            return h.ComputeHash(stream);
        }
    }
}