using Xunit;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class Noark5TestSelectionFileReaderTests
    {
        [Fact]
        public void ParseFileContentTest()
        {
            var content = new[] {"N5.13", "#N5.27", "N5.63", "N5.ab", "A.15"};

            List<TestId> testIds = Noark5TestSelectionFileReader.ParseFileContent(content);

            testIds.Should().Contain(TestId.Create("N5.13"));
            testIds.Should().Contain(TestId.Create("N5.63"));
            testIds.Should().NotContain(TestId.Create("N5.27"));
            testIds.Should().NotContain(TestId.Create("A.15"));
        }
    }
}
