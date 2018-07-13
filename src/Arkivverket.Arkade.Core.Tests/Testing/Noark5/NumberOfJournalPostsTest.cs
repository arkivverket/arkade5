using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfJournalPostsTest
    {
        [Fact]
        public void EqualNumbersInArchiveAndJournalsIsAlwaysOk()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SharpSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstrukturen: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void UnEqualNumbersInJournalsIsNeverOk()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SoftSeparationAndUnEqualJournals";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antallet journalposter i offentlig og løpende journal er ulikt"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstrukturen: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void
            NumbersInArchiveThatIsDifferentFromNumbersInJournalsIsNotOkWithSharpSeparation() // Should contain errors
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

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SharpSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = helper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Periodeskille er skarpt og antallet journalposter i arkivstrukturen er ikke likt det i offentlig og løpende journal"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstrukturen: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void
            NumbersInArchiveThatIsDifferentFromNumbersInJournalsIsOkWithSoftSeparation() 
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

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SoftSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = helper.RunEventsOnTest(new NumberOfJournalPosts(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter funnet i arkivstrukturen: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i løpende journal: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall journalposter dokumentert i offentlig journal: 4"));
            testRun.Results.Count.Should().Be(3);
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
