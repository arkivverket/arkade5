using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfEachCaseFolderStatusTest
    {
        [Fact]
        public void ShouldFindSeveralCaseFolderStatusesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Utgår"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Under behandling"))))));


            TestRun testRun = helper.RunEventsOnTest(new NumberOfEachCaseFolderStatus());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Saksmappestatus: Avsluttet - Antall: 2"
                ));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Saksmappestatus: Utgår - Antall: 1"
                ));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Saksmappestatus: Under behandling - Antall: 1"
                ) && r.IsError()); // Only "Avsluttet" or "Utgår" on regular deposits

            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldFindSeveralCaseFolderStatusesInSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet")))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))))));


            TestRun testRun = helper.RunEventsOnTest(new NumberOfEachCaseFolderStatus());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID): someArchivePartSystemId_1 - Saksmappestatus: Avsluttet - Antall: 1"
                ));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID): someArchivePartSystemId_2 - Saksmappestatus: Avsluttet - Antall: 1"
                ));

            testRun.Results.Count.Should().Be(2);
        }
    }
}
