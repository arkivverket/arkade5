using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_38_NumberOfPrecedentsTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfPrecedentsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                    ))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_38_NumberOfPrecedents());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NumberOfPrecedentsInJournalpostsIsOneInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()))
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_38_NumberOfPrecedents());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 1");
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Antall presedenser i journalposter: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 1");
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Antall presedenser i journalposter: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void NumberOfPrecedents()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()))
                                    .Add("registrering", new[] { "xsi:type", "journalpost" }, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()
                                        )))))))
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_2")
                    .Add("tittel", "someTitle_2")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                .Add("presedens", new XmlElementHelper()
                                )))));

            TestRun testRun = helper.RunEventsOnTest(new N5_38_NumberOfPrecedents());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 3");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 2");
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Antall presedenser i journalposter: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 1");
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Antall presedenser i saksmapper: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void NumberOfPrecedentsInArchiveWithOneArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] { "xsi:type", "journalpost" }, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()))
                                    .Add("registrering", new[] { "xsi:type", "journalpost" }, new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_38_NumberOfPrecedents());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 2");
            testResults.Should().Contain(r => r.Message.Equals("Antall presedenser i journalposter: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }
    }
}