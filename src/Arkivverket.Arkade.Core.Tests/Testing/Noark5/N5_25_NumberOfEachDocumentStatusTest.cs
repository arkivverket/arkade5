using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_25_NumberOfEachDocumentStatusTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralDocumentStatusesInSingleArchivePart()
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
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er under redigering"))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_25_NumberOfEachDocumentStatus());

            testRun.Results.Should()
                .Contain(r =>
                            r.Message.Equals("Dokumentstatus: Dokumentet er ferdigstilt - Antall: 2")
                );
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals("Dokumentstatus: Dokumentet er under redigering - Antall: 1") &&
                        r.IsError() // Only "Dokumentet er ferdigstilt" on regular deposits
                );
            testRun.Results.Count.Should().Be(2);
        }

        [Fact]
        public void ShouldFindSeveralDocumentStatusesInSeveralArchiveParts()
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
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er under redigering")))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er ferdigstilt"))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentstatus", "Dokumentet er under redigering"))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_25_NumberOfEachDocumentStatus());

            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Dokumentstatus: Dokumentet er ferdigstilt - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Dokumentstatus: Dokumentet er under redigering - Antall: 1") &&
                    r.IsError() // Only "Dokumentet er ferdigstilt" on regular deposits
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Dokumentstatus: Dokumentet er ferdigstilt - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Dokumentstatus: Dokumentet er under redigering - Antall: 1") &&
                    r.IsError() // Only "Dokumentet er ferdigstilt" on regular deposits
            );
            testRun.Results.Count.Should().Be(4);
        }
    }
}
