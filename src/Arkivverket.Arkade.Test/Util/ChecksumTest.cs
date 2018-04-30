using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Util
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
            var pathToFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Util/demo.txt";
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
                var pathToFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Util/demo2.txt";
                new Sha256ChecksumGenerator().GenerateChecksum(pathToFile);
            });
        }
    }
}
