using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfArchivesTest
    {
        [Fact]
        public void NumberOfArchivesIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper()
                        .Add("tittel", "Dette er tittelen")
                        .Add("arkivdelstatus", "Avsluttet")
                ));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfArchives());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfArchivesIsThree()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper()
                        .Add("arkiv", string.Empty)
                        .Add("arkiv", string.Empty)
                ));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfArchives());

            testRun.Results.First().Message.Should().Contain("3");
        }
    }
}