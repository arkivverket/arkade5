using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_15_NumberOfEachCaseFolderStatusTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralCaseFolderStatusesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Utgår"))
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Under behandling"))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_15_NumberOfEachCaseFolderStatus());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 3");
            testResults.Should().Contain(r => r.Message.Equals("Saksmappestatus: Avsluttet - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Saksmappestatus: Utgår - Antall: 1"));
            testResults.Should().Contain(r =>
                r.Message.Equals("Saksmappestatus: Under behandling - Antall: 1") &&
                r.IsError()); // Only "Avsluttet" or "Utgår" on regular deposits

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void ShouldFindSeveralCaseFolderStatusesInSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("tittel", "someArchivePartTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet")))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_2")
                        .Add("tittel", "someArchivePartTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("saksstatus", "Avsluttet"))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_15_NumberOfEachCaseFolderStatus());

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.First().Message.Should().Be("Antall: 1");
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Saksmappestatus: Avsluttet - Antall: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.First().Message.Should().Be("Antall: 1");
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Saksmappestatus: Avsluttet - Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void ShouldFindNoCaseFolderStatusesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("tittel", "someArchivePartTitle_1")));


            TestRun testRun = helper.RunEventsOnTest(new N5_15_NumberOfEachCaseFolderStatus());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
