using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_29_NumberOfEachDocumentFormatTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralDocumentFormatsInSingleArchivePart()
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
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename2.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "docx")
                                                                        .Add("referanseDokumentfil", "filename3.docx")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename4.docx")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_29_NumberOfEachDocumentFormat());

            testRun.Results.Should()
                .Contain(r =>
                            r.Message.Equals("Dokumentformat: pdf - Antall: 2")
                );
            testRun.Results.Should()
                .Contain(r =>
                            r.Message.Equals("Dokumentformat: docx - Antall: 1")
                );
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals(
                            "Format-misforhold: Dokumentformat: pdf - Dokumentfilreferanse: filename4.docx") &&
                        r.IsError()
                );
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldFindSeveralDocumentFormatsInSeveralArchiveParts()
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
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename2.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "docx")
                                                                        .Add("referanseDokumentfil", "filename3.docx")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename4.docx"))))))))
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
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename5.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename6.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "docx")
                                                                        .Add("referanseDokumentfil", "filename7.docx")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering",
                                                    new[] {"xsi:type", "journalpost"},
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("format", "pdf")
                                                                        .Add("referanseDokumentfil", "filename8.docx")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_29_NumberOfEachDocumentFormat());

            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Dokumentformat: pdf - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Dokumentformat: docx - Antall: 1")
            );
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals(
                            "Format-misforhold: Dokumentformat: pdf - Dokumentfilreferanse: filename4.docx") &&
                        r.IsError()
                );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Dokumentformat: pdf - Antall: 2")
            );
            testRun.Results.Should().Contain(r =>
                    r.Message.Equals(
                        "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Dokumentformat: docx - Antall: 1")
            );
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals(
                            "Format-misforhold: Dokumentformat: pdf - Dokumentfilreferanse: filename8.docx") &&
                        r.IsError()
                );
            testRun.Results.Count.Should().Be(6);
        }
    }
}
