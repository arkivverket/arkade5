using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_64_NumberOfEmptyDocumentFilesTest : LanguageDependentTest
    {
        [Fact]
        public void TestTypeShouldBeContentAnalysis()
        {
            new N5_64_NumberOfEmptyDocumentFiles(null).GetTestType().Should().Be(TestType.ContentAnalysis);
        }

        [Fact]
        public void EmptyDocumentFilesAreReported()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("registreringsID", "regId_1")
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "sysId_1")
                                            .Add("dokumentnummer", "1")
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/empty.txt")
                                                .Add("filstoerrelse", "0")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(2);

            TestResult testResult = testRun.TestResults.TestsResults[1];
            testResult.Message.Should().Be(
                "Filen dokumenter/empty.txt er tom. Dokumentbeskrivelse (systemID, registreringsID, dokumentnummer): sysId_1, regId_1, 1"
            );
        }

        [Fact]
        public void EmptyDocumentFileWithDocumentedSizeNotZeroIsReported()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("registreringsID", "regId_1")
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "sysId_1")
                                            .Add("dokumentnummer", "1")
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/empty.txt")
                                                .Add("filstoerrelse", "1")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(2);

            TestResult testResult = testRun.TestResults.TestsResults[1];
            testResult.Message.Should().Be(
                "Filen dokumenter/empty.txt er tom. Dokumentbeskrivelse (systemID, registreringsID, dokumentnummer): sysId_1, regId_1, 1"
            );
        }

        [Fact]
        public void NonEmptyDocumentFilesAreNotReported()
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
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                .Add("filstoerrelse", "20637")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Theory]
        [InlineData("utgår", "avsluttet", "arkivert")]
        [InlineData("avsluttet", "utgår", "arkivert")]
        [InlineData("avsluttet", "avsluttet", "utgår")]
        public void EmptyDocumentFileSubjectToStatusUtgaarIsNotReported(string superFolderStatus, string folderStatus, string regStatus)
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("dokumentobjekt", new XmlElementHelper()
                                                    .Add("referanseDokumentfil", "dokumenter/empty.txt")
                                                    .Add("filstoerrelse", "0")))
                                            .Add("journalstatus", regStatus))
                                        .Add("saksstatus", folderStatus))
                                    .Add("saksstatus", superFolderStatus))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void OneEmptyDocumentFileInOneOfTwoArchivePartsIsReported()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_1")
                    .Add("tittel", "someTitle_1")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("registreringsID", "regId_1")
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("systemID", "sysId_1")
                                        .Add("dokumentnummer", "1")
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("referanseDokumentfil", "dokumenter/empty.txt")
                                            .Add("filstoerrelse", "0"))))))))
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_2")
                    .Add("tittel", "someTitle_2")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                            .Add("filstoerrelse", "20637")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            // Summarized
            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 1");

            // First archive part (systemID: someSystemId_1)
            testRun.TestResults.TestResultSets[0].TestsResults[0].Message.Should().Be("Totalt: 1");
            testRun.TestResults.TestResultSets[0].TestsResults[1].Message.Should().Be(
                "Filen dokumenter/empty.txt er tom. Dokumentbeskrivelse (systemID, registreringsID, dokumentnummer): sysId_1, regId_1, 1"
            );

            // Second archive part (systemID: someSystemId_2)
            testRun.TestResults.TestResultSets[1].TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void FilePathsWithBackOrForwardSlashAreParsed()
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
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                .Add("filstoerrelse", "20637")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter\\5000000.pdf")
                                                .Add("filstoerrelse", "20637")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void DocumentedAndActualFileSizeMismatchIsNotReported()
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
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                .Add("filstoerrelse", "0")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NonExistingFileIsNotReported()
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
                                                .Add("referanseDokumentfil", "non-existing.file")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "EmptyFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_64_NumberOfEmptyDocumentFiles(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
