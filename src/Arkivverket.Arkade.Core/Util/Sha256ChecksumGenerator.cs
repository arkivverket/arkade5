using System;
using System.IO;
using System.Security.Cryptography;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public class Sha256ChecksumGenerator : IChecksumGenerator
    {
        private HashAlgorithm _hasher;

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

        public void Initialize()
        {
            _hasher = (HashAlgorithm)CryptoConfig.CreateFromName("SHA256");
            _hasher?.Initialize();
        }

        /// <summary>
        /// Must be called after <see cref="Initialize"/>. Will otherwise throw a <see cref="NullReferenceException"/>.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="numberOfBytes"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void TransformBlock(byte[] buffer, int numberOfBytes)
        {
            CheckIfHasherIsInitialized();

            _hasher.TransformBlock(buffer, 0, numberOfBytes, null, 0);
        }

        /// <summary>
        /// Must be called after <see cref="Initialize"/>. Will otherwise throw a <see cref="NullReferenceException"/>.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public string GenerateChecksum()
        {
            CheckIfHasherIsInitialized();

            _hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return Hex.ToHexString(_hasher.Hash);
        }

        private void CheckIfHasherIsInitialized()
        {
            if (_hasher == null)
                throw new NullReferenceException(
                    $"Initialize() method of {nameof(Sha256ChecksumGenerator)} must be called before attempting to compute the hash for the buffer.");
        }
    }
}