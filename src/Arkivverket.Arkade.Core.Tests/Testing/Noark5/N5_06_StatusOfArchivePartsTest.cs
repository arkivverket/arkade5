using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_06_StatusOfArchivePartsTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindStatusOfSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("arkivdelstatus", "someStatus_1")
                    ));

            TestRun testRun = helper.RunEventsOnTest(new N5_06_StatusOfArchiveParts());

            testRun.Results.First().Message.Should().Be("Arkivdelstatus: someStatus_1");
        }

        [Fact]
        public void ShouldFindStatusOfSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("arkivdelstatus", "someStatus_1")
                    )
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("arkivdelstatus", "someStatus_2")
                    ));

            TestRun testRun = helper.RunEventsOnTest(new N5_06_StatusOfArchiveParts());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Status for arkivdel (systemID, tittel) someSystemId_1, someTitle_1: someStatus_1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Status for arkivdel (systemID, tittel) someSystemId_2, someTitle_2: someStatus_2"));
        }
    }
}
