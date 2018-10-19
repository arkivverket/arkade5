using System;

namespace Arkivverket.Arkade.Core.Util
{
    public class ChecksumGeneratorFactory
    {

        public IChecksumGenerator GetGenerator(string algorithm)
        {
            if (algorithm == "SHA-256" || algorithm == "SHA256")
            {
                return new Sha256ChecksumGenerator();
            }
            throw new ArgumentException($"Checksum algorithm not supported: {algorithm}");
        }


    }
}
