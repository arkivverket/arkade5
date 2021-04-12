using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_60_ArchiveStartAndEndDateControlTest : LanguageDependentTest
    {
        [Fact]
        public void EqualDatesInArchiveAndJournalsIsAlwaysOk()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SharpSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_60_ArchiveStartAndEndDateControl(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i arkivstrukturen: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i offentlig journal: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i løpende journal: 09.09.2011 - 10.10.2012"));
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void UnEqualDatesInJournalsIsNeverOk() // Should contain errors
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory =
                "TestData\\Noark5\\JournalControl\\SoftSeparationAndUnEqualJournals";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_60_ArchiveStartAndEndDateControl(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i arkivstrukturen: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i offentlig journal: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i løpende journal: 09.09.2010 - 10.10.2013")); // Different
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Datoer i offentlig og løpende journal er ikke like") && r.IsError());
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void DatesInArchiveThatIsDifferentFromDatesInJournalsIsOkWithSoftSeparation()
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations();

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SoftSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_60_ArchiveStartAndEndDateControl(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i arkivstrukturen: 09.09.2011 - 10.10.2012")); // Different
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i offentlig journal: 09.09.2010 - 10.10.2013"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i løpende journal: 09.09.2010 - 10.10.2013"));
            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void DatesInArchiveThatIsDifferentFromDatesInJournalsIsNotOkWithSharpSeparation() // Should contain errors
        {
            XmlElementHelper xmlElementHelper = MockUp4JournalPostRegistrations("2013-10-10"); // Adjusted last date

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SharpSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_60_ArchiveStartAndEndDateControl(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i arkivstrukturen: 09.09.2011 - 10.10.2013")); // Different
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i offentlig journal: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Start- og sluttdato i løpende journal: 09.09.2011 - 10.10.2012"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Periodeskille er skarpt og datoer i arkivstrukturen er ikke like de i offentlig og løpende journal") && r.IsError());
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void NoJournalDatesInArchiveIsHandled() // Should contain errors
        {
            XmlElementHelper xmlElementHelper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper().Add("klasse",
                            new XmlElementHelper().Add("mappe",
                                new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                    "no journal date"))))));

            const string testdataDirectory = "TestData\\Noark5\\JournalControl\\SharpSeparation";

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_60_ArchiveStartAndEndDateControl(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det ble ikke funnet noen gyldige journaldatoer i arkivuttrekket"));
            testRun.Results.Count.Should().Be(1);
        }

        private static XmlElementHelper MockUp4JournalPostRegistrations(string lastDate = "2012-10-10")
        {
            return new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper().Add("klasse",
                            new XmlElementHelper().Add("mappe",
                                new XmlElementHelper()
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper().Add("journaldato", "2011-09-09"))
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper().Add("journaldato", "2011-10-10"))
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper().Add("journaldato", "2012-09-09"))
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper().Add("journaldato", lastDate)))))));
        }
    }
}
