using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_22_NumberOfEachJournalStatusTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralJournalStatusesInSingleArchivePart()
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
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Journalstatus: Arkivert - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Journalstatus: Utgår - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Journalstatus: JournalFørt - Antall: 1")
                                              && r.IsError()); // Only "Arkivert" or "Utgår" on regular deposits

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldFindSeveralJournalStatusesInSeveralArchiveParts()
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
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt"))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Arkivert")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "Utgår")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus", "JournalFørt")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalstatus: Arkivert - Antall: 2"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalstatus: Utgår - Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalstatus: JournalFørt - Antall: 1")
                                                   && r.IsError());

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Journalstatus: Arkivert - Antall: 2"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Journalstatus: Utgår - Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Journalstatus: JournalFørt - Antall: 1")
                                                   && r.IsError());

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void ShouldHandleEmptyOrAbsentJournalStatuses()
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
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper().Add("journalstatus",
                                                    string.Empty))) // Status empty
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                string.Empty)))))); // No status element

            TestRun testRun = helper.RunEventsOnTest(new N5_22_NumberOfEachJournalStatus());

            testRun.TestResults.TestResultSets.Find(s => s.Name.Equals("Journalstatus: "))
                ?.TestsResults.Should().Contain(r => r.Message.Equals("Antall: 1") && r.IsError());

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
