using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5.Structure;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateChecksumsTest
    {

        [Fact]
        public void ShouldValidateThatAllChecksumsAreCorrect()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\correct";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);

            new ValidateChecksums().RunTest(archiveExtraction);


        }

    }
}
