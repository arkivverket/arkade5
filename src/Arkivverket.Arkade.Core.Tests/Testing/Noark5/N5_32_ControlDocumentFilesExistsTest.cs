using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_32_ControlDocumentFilesExistsTest : LanguageDependentTest
    {
        [Fact]
        public void ReferencedFilesExists()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf"))))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void ReferencedFilesAreMissing()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000002.pdf"))))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000002.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Filen dokumenter/5000002.pdf ble ikke funnet"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Filen dokumenter/5000002.pdf ble ikke funnet"));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void ReferencedFilesAreMissingFromArchiveWithOneArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000002.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Filen dokumenter/5000002.pdf ble ikke funnet"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void SlashOrBackSlashDirectorSeparatorIsIndifferent()
        {
            XmlElementHelper helper = new XmlElementHelper()
               .Add("arkiv", new XmlElementHelper()
                   .Add("arkivdel", new XmlElementHelper()
                       .Add("systemID", "someSystemId_1")
                       .Add("klassifikasjonssystem", new XmlElementHelper()
                           .Add("klasse", new XmlElementHelper()
                               .Add("mappe", new XmlElementHelper()
                                   .Add("registrering", new XmlElementHelper()
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter\\5000001.pdf"))))))))
                   .Add("arkivdel", new XmlElementHelper()
                       .Add("systemID", "someSystemId_2")
                       .Add("klassifikasjonssystem", new XmlElementHelper()
                           .Add("klasse", new XmlElementHelper()
                               .Add("mappe", new XmlElementHelper()
                                   .Add("registrering", new XmlElementHelper()
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter\\5000000.pdf")))
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }
    }
}
