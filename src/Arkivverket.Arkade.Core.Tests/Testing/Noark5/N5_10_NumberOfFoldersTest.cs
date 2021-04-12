using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
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

            testRun.Results.Count.Should().Be(3); // "Totalt" added

            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: saksmappe - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: moetemappe - Antall: 1"));
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

            testRun.Results.Count.Should().Be(9); // "Totalt" added

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Mappetype: saksmappe - Antall: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Mappetype: saksmappe - Antall på nivå 1: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Mappetype: saksmappe - Antall på nivå 2: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Mappetype: moetemappe - Antall: 1"));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Mappetype: saksmappe - Antall: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Mappetype: saksmappe - Antall på nivå 1: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Mappetype: saksmappe - Antall på nivå 2: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Mappetype: moetemappe - Antall: 1"));
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er angitt at arkivstrukturen skal innholde 2 mapper men 1 ble funnet"
            ));
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

            testRun.Results.Count.Should().Be(3);

            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: saksmappe - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: mappe - Antall: 1"));
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

            testRun.Results.Count.Should().Be(3);

            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: saksmappe - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Mappetype: minmappe - Antall: 1"));
        }

    }
}
