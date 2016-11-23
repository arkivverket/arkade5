using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class StatusOfArchivePartsTest
    {
        [Fact]
        public void ShouldReturnStatusOfAllArchiveParts()
        {
            const string title1 = "Dette er tittel 1";
            const string status1 = "Avsluttet";
            const string title2 = "Dette er tittel 2";
            const string status2 = "Startet";

            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("tittel", title1)
                            .Add("arkivdelstatus", status1)
                    )
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("tittel", title2)
                            .Add("arkivdelstatus", status2)
                    ));

            TestRun testRun = helper.RunEventsOnTest(new StatusOfArchiveParts());

            string message = testRun.Results.First().Message;
            message.Should().Contain(title1 + ": " + status1);
            message.Should().Contain(title2 + ": " + status2);
        }
    }
}