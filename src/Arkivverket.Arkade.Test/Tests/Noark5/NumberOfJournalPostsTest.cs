using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfJournalPostsTest
    {
        [Fact]
        public void NoDeviations()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalPosts\\running4public4";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstruktur: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void RunningJournalDeviates()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalPosts\\running3public4";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er ikke samsvar mellom dokumentert antall og faktisk antall journalposter"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstruktur: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void PublicJournalDeviates()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalPosts\\running4public3";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er ikke samsvar mellom dokumentert antall og faktisk antall journalposter"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstruktur: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 3"));
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void JournalFilesIsMissing()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalPosts\\doesntexist";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "En eller flere journalfiler mangler"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er ikke samsvar mellom dokumentert antall og faktisk antall journalposter"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstruktur: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 0"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 0"));
            testRun.Results.Count.Should().Be(5);
        }

        [Fact]
        public void ArchiveExtractionDeviates()
        {
            // Mock up 3 journalpost registrations:
            var helper = new XmlElementHelper();
            helper.Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper().Add("klasse",
                            new XmlElementHelper()
                                .Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... ")
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... ")
                                        .Add("registrering", " ... ") // No journalpost attribute
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... "))))));

            const string testdataDirectory = "TestData\\Noark5\\JournalPosts\\running4public4";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = helper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er ikke samsvar mellom dokumentert antall og faktisk antall journalposter"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstruktur: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(4);
        }

        private static XmlElementHelper MockUp4JournalPostRegistrations()
        {
            return new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper().Add("klasse",
                            new XmlElementHelper()
                                .Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... ")
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... ")
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... ")
                                        .Add("registrering", new[] {"xsi:type", "journalpost"}, " ... "))))));
        }
    }
}