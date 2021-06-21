using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;


namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_35_NumberOfCasePartsTests : LanguageDependentTest
    {
        const string TestDataDirectory = "TestData\\Noark5\\Small";
        const string TestDataDirectoryV5_5 = "TestData\\Noark5\\Version5_5";

        [Fact]
        public void NumberOfCasePartsIsOne()
        {
            Archive testArchive = TestUtil.CreateArchiveExtraction(TestDataDirectory);
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "archivePartSystemId_1")
                        .Add("tittel", "archivePartTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NumberOfCasePartsIsOnePerArchivePart()
        {
            Archive testArchive = TestUtil.CreateArchiveExtraction(TestDataDirectory);
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "journpost57d6608569ed33.70652483"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_3")
                        .Add("tittel", "someTitle_3")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults[0].Message.Should().Be("Totalt: 2");
            testResults[1].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1: 1");
            testResults[2].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2: 0");
            testResults[3].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_3, someTitle_3: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void NumberOfCasePartsIsZero()
        {
            Archive testArchive = TestUtil.CreateArchiveExtraction(TestDataDirectory);
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "archivePartSystemId_1")
                        .Add("tittel", "archivePartTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NumberOfCasePartsIsOneNoark5_5()
        {
            Archive testArchive = TestUtil.CreateArchiveExtractionV5_5(TestDataDirectoryV5_5);

            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("part", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }


        [Fact]
        public void NumberOfCasePartsIsOnePerArchivePartNoark5_5()
        {
            Archive testArchive = TestUtil.CreateArchiveExtractionV5_5(TestDataDirectoryV5_5);
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("part", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "journpost57d6608569ed33.70652483"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_3")
                        .Add("tittel", "someTitle_3")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("part", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults[0].Message.Should().Be("Totalt: 2");
            testResults[1].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1: 1");
            testResults[2].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2: 0");
            testResults[3].Message.Should().Be("Arkivdel (systemID, tittel): someSystemId_3, someTitle_3: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void NumberOfCasePartsIsZeroNoark5_5()
        {
            Archive testArchive = TestUtil.CreateArchiveExtractionV5_5(TestDataDirectoryV5_5);

            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts(testArchive));

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}