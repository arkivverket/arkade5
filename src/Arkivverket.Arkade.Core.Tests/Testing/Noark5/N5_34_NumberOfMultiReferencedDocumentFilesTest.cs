using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Arkivverket.Arkade.Core.Testing;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_34_NumberOfMultiReferencedDocumentFilesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindMultiReferencedDocumentFilesInSingleArchivePart()
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
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename2.docx")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_34_NumberOfMultiReferencedDocumentFiles());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 1");

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r =>
                r.Message.Equals("Referert dokumentfil: filename1.pdf - Antall referanser: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void ShouldFindMultiReferencedDocumentFilesInSeveralArchiveParts()
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
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename2.docx"))))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
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
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename3.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename3.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename4.docx"))))))))
            );


            TestRun testRun = helper.RunEventsOnTest(new N5_34_NumberOfMultiReferencedDocumentFiles());


            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.First().Message.Should().Be("Antall: 1");
            arkivdel1Results.Should().Contain(r =>
                r.Message.Equals("Referert dokumentfil: filename1.pdf - Antall referanser: 2"));


            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.First().Message.Should().Be("Antall: 1");
            arkivdel2Results.Should().Contain(
                r => r.Message.Equals("Referert dokumentfil: filename3.pdf - Antall referanser: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void MultiReferencedDocumentFilesIsZero()
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
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename2.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename3.docx")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_34_NumberOfMultiReferencedDocumentFiles());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");
            
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
