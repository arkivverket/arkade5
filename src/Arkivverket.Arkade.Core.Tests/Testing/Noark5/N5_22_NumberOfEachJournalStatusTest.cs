using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_22_NumberOfEachJournalStatusTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralJournalStatusesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            testRun.Results.Should().Contain(r => r.Message.Equals("Journalstatus: Arkivert - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Journalstatus: Utgår - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Journalstatus: JournalFørt - Antall: 1") &&
                                                  r.IsError()); // Only "Arkivert" or "Utgår" on regular deposits
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldFindSeveralJournalStatusesInSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("tittel", "someArchivePartTitle_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt"))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Journalstatus: Arkivert - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Journalstatus: Utgår - Antall: 1")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Journalstatus: JournalFørt - Antall: 1") &&
                    r.IsError() // Only "Arkivert" or "Utgår" on regular deposits
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Journalstatus: Arkivert - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Journalstatus: Utgår - Antall: 1")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Journalstatus: JournalFørt - Antall: 1") &&
                    r.IsError() // Only "Arkivert" or "Utgår" on regular deposits
            );
            testRun.Results.Count.Should().Be(6);
        }

        [Fact]
        public void ShouldHandleEmptyOrAbsentJournalStatuses()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus",
                                                    string.Empty))) // Status empty
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                string.Empty)))))); // No status element

            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            testRun.Results.Should().Contain(r => r.Message.Equals("Journalstatus:  - Antall: 1") && r.IsError());

            testRun.Results.Count.Should().Be(1);
        }
    }
}
