using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_10_NumberOfFoldersTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindOneCaseFolderAndOneMeetingfolder()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper())
                            .Add("mappe", new[] {"xsi:type", "moetemappe"}, new XmlElementHelper()))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "TwoFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            TestResultSet saksmappeResultSet = testRun.TestResults.TestResultSets[0];
            saksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            TestResultSet moetemappeResultSet = testRun.TestResults.TestResultSets[1];
            moetemappeResultSet.Name.Should().Be("Mappetype: moetemappe");
            moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldFindTwoCaseFoldersAndOneMeetingFolderInBothOfTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()))
                            .Add("mappe", new[] {"xsi:type", "moetemappe"}, new XmlElementHelper()
                            )))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()))
                            .Add("mappe", new[] {"xsi:type", "moetemappe"}, new XmlElementHelper()
                            ))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "SixFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 6"));

            TestResultSet arkivdel1ResultSet = testRun.TestResults.TestResultSets[0];
            arkivdel1ResultSet.Name.Should().Be("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1");
            arkivdel1ResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 3"));

            TestResultSet arkivdel1SaksmappeResultSet = arkivdel1ResultSet.TestResultSets[0];
            arkivdel1SaksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 2"));
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 2: 1"));

            TestResultSet arkivdel1moetemappeResultSet = arkivdel1ResultSet.TestResultSets[1];
            arkivdel1moetemappeResultSet.Name.Should().Be("Mappetype: moetemappe");
            arkivdel1moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            arkivdel1moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));


            TestResultSet arkivdel2ResultSet = testRun.TestResults.TestResultSets[1];
            arkivdel2ResultSet.Name.Should().Be("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2");
            arkivdel2ResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 3"));

            TestResultSet arkivdel2SaksmappeResultSet = arkivdel2ResultSet.TestResultSets[0];
            arkivdel2SaksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            arkivdel2SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 2"));
            arkivdel2SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));
            arkivdel2SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 2: 1"));

            TestResultSet arkivdel2moetemappeResultSet = arkivdel2ResultSet.TestResultSets[1];
            arkivdel2moetemappeResultSet.Name.Should().Be("Mappetype: moetemappe");
            arkivdel2moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            arkivdel2moetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(13);
        }

        [Fact]
        public void DocumentedAndFoundNumberOfFoldersMismatchShouldTriggerWarning()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "TwoFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 1"));
            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Det er angitt at arkivstrukturen skal innholde 2 mapper men 1 ble funnet"
            ));

            List<TestResult> saksmappeResults = testRun.TestResults.TestResultSets[0].TestsResults;
            saksmappeResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            saksmappeResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void ShouldFindOneCaseFolderAndOneFolderWithoutType()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper())
                            .Add("mappe", /* No specific type */ new XmlElementHelper()))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "TwoFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            TestResultSet saksmappeResultSet = testRun.TestResults.TestResultSets[0];
            saksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            TestResultSet mappeResultSet = testRun.TestResults.TestResultSets[1];
            mappeResultSet.Name.Should().Be("Mappetype: mappe");
            mappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            mappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldFindOneCaseFolderAndOneCustomfolder()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper())
                            .Add("mappe", new[] { "xsi:type", "minmappe" }, new XmlElementHelper()))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "TwoFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            TestResultSet saksmappeResultSet = testRun.TestResults.TestResultSets[0];
            saksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            saksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            TestResultSet customFolderResultSet = testRun.TestResults.TestResultSets[1];
            customFolderResultSet.Name.Should().Be("Mappetype: minmappe");
            customFolderResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            customFolderResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldNotFindAnyFolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper())));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "NoFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldFindTwoCaseFoldersAndOneMeetingFolderInOneOfTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()))
                            .Add("mappe", new[] { "xsi:type", "moetemappe" }, new XmlElementHelper()
                            )))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper())));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "FolderControl", "SixFolders")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_10_NumberOfFolders(testArchive));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 3"));
            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Det er angitt at arkivstrukturen skal innholde 6 mapper men 3 ble funnet"
            ));

            TestResultSet arkivdel1ResultSet = testRun.TestResults.TestResultSets[0];
            arkivdel1ResultSet.Name.Should().Be("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1");
            arkivdel1ResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 3"));

            TestResultSet arkivdel1SaksmappeResultSet = arkivdel1ResultSet.TestResultSets[0];
            arkivdel1SaksmappeResultSet.Name.Should().Be("Mappetype: saksmappe");
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 2"));
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));
            arkivdel1SaksmappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 2: 1"));

            TestResultSet arkivdel1MoetemappeResultSet = arkivdel1ResultSet.TestResultSets[1];
            arkivdel1MoetemappeResultSet.Name.Should().Be("Mappetype: moetemappe");
            arkivdel1MoetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1"));
            arkivdel1MoetemappeResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall på nivå 1: 1"));


            TestResultSet arkivdel2ResultSet = testRun.TestResults.TestResultSets[1];
            arkivdel2ResultSet.Name.Should().Be("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2");
            arkivdel2ResultSet.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(9);
        }

    }
}
