using System.IO;
using System.Security.Cryptography;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Util
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
            HashAlgorithm h = (HashAlgorithm) CryptoConfig.CreateFromName("SHA256");
            if (h == null)
            {
                throw new ArkadeException("Checksum algorithm SHA-256 not supported.");
            }

            byte[] bytes;
            using (FileStream fs = new FileInfo(pathToFile).OpenRead())
            {
                bytes = h.ComputeHash(fs);
            }
            return Hex.ToHexString(bytes);
        }
    }
}