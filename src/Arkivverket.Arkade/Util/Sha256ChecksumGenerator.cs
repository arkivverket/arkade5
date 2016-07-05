using System;
using System.IO;
using System.Security.Cryptography;

namespace Arkivverket.Arkade.Util
{
    public class Sha256ChecksumGenerator : IChecksumGenerator
    {
        /// <summary>
        /// Generates a checksum with the SHA-256 algorithm
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public string GenerateChecksum(string pathToFile) {
            HashAlgorithm hashUtil = new SHA256CryptoServiceProvider();
            var hash = hashUtil.ComputeHash(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

    }
}
