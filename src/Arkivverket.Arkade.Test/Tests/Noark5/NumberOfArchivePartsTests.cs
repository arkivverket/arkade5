using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfArchivePartsTests
    {
        [Fact]
        public void ShouldReturnTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", string.Empty)
                    .Add("arkivdel", string.Empty)
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfArchiveParts());

            testRun.Results.First().Message.Should().Contain("2");
        }
    }
}