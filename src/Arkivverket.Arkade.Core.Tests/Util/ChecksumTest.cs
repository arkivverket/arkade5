using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class ChecksumTest
    {
        private readonly ITestOutputHelper _output;

        public ChecksumTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GenerateChecksum()
        {
            var pathToFile = AppDomain.CurrentDomain.BaseDirectory + "\\Util\\demo.txt";
            _output.WriteLine("File path: " + pathToFile);

            string checksum = new Sha256ChecksumGenerator().GenerateChecksum(pathToFile);
            checksum.Should().Be("8661245AF5506ABE3347B7906A54EDCAE280EC8638E6801A373A948AD2A35D96"); // generated with another tool 

            _output.WriteLine("Checksum: " + checksum);
        }

        [Fact]
        public void CheckForFileExistence()
        {
            Xunit.Assert.Throws<FileNotFoundException>(delegate
            {
                var pathToFile = AppDomain.CurrentDomain.BaseDirectory + "\\Util\\demo2.txt";
                new Sha256ChecksumGenerator().GenerateChecksum(pathToFile);
            });
        }
    }
}
