using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class Noark5TestFactoryTests
    {
        [Fact]
        public void AllNoark5TestsAreFabricatedWithTheCorrectTestId()
        {
            Archive archive = TestUtil.CreateArchiveExtraction(Path.Combine("TestData", "Noark5", "Noark5Archive"));

            var noark5TestFactory = new Noark5TestFactory(archive);

            foreach (TestId testId in noark5TestFactory.GetTestIds())
            {
                noark5TestFactory.Create(testId).GetId().Should().Be(testId);
            }
        }
    }
}
