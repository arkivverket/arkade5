using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
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

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt antall unike formater: 2");
            testResults.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1")
                && r.IsError());

            List<TestResult> pdfFormatResults =
                testRun.TestResults.TestResultSets.Find(s => s.Name.Equals("Dokumentformat: pdf"))?.TestsResults;
            pdfFormatResults.Should().Contain(r => r.Message.Equals("Antall: 3"));
            pdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1") && r.IsError()
            );
            pdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Format-misforhold: Dokumentfilreferanse: filename4.docx") &&
                r.IsError()
            );

            List<TestResult> arkivdel1docxFormatResults =
                testRun.TestResults.TestResultSets.Find(s => s.Name.Equals("Dokumentformat: docx"))?.TestsResults;
            arkivdel1docxFormatResults.Should().Contain(r => r.Message.Equals("Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
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

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt antall unike formater: 2");
            testResults.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 2") &&
                r.IsError());

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1") && r.IsError()
            );

            List<TestResultSet> arkivdel1ResultSets = testRun.TestResults.TestResultSets[0].TestResultSets;

            List<TestResult> arkivdel1PdfFormatResults =
                arkivdel1ResultSets.Find(s => s.Name.Equals("Dokumentformat: pdf"))?.TestsResults;
            arkivdel1PdfFormatResults.Should().Contain(r => r.Message.Equals("Antall: 3"));
            arkivdel1PdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1") && r.IsError()
            );
            arkivdel1PdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Format-misforhold: Dokumentfilreferanse: filename4.docx") &&
                r.IsError()
            );

            List<TestResult> arkivdel1DocxFormatResults =
                arkivdel1ResultSets.Find(s => s.Name.Equals("Dokumentformat: docx"))?.TestsResults;
            arkivdel1DocxFormatResults.Should().Contain(r => r.Message.Equals("Antall: 1"));


            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1") && r.IsError()
            );

            List<TestResultSet> arkivdel2ResultSets = testRun.TestResults.TestResultSets[1].TestResultSets;

            List<TestResult> arkivdel2PdfFormatResults =
                arkivdel2ResultSets.Find(s => s.Name.Equals("Dokumentformat: pdf"))?.TestsResults;
            arkivdel2PdfFormatResults.Should().Contain(r => r.Message.Equals("Antall: 3"));
            arkivdel2PdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Antall dokumenter med format-misforhold: 1") && r.IsError()
            );
            arkivdel2PdfFormatResults.Should().Contain(r =>
                r.Message.Equals("Format-misforhold: Dokumentfilreferanse: filename8.docx") &&
                r.IsError()
            );

            List<TestResult> arkivdel2DocxFormatResults =
                arkivdel2ResultSets.Find(s => s.Name.Equals("Dokumentformat: docx"))?.TestsResults;
            arkivdel2DocxFormatResults.Should().Contain(r => r.Message.Equals("Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(12);
        }

        [Fact]
        public void ShouldFindNoDocumentFormats()
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
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_29_NumberOfEachDocumentFormat());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt antall unike formater: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
