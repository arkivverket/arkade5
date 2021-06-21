using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_36_NumberOfCommentsTests : LanguageDependentTest
    {


        [Fact]
        public void CommentsAreFoundUnderCaseFolderBaseRegistrationJournalPostAndDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                    .Add("merknad", string.Empty)
                                    .Add("merknad", string.Empty))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("merknad", string.Empty)))
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                    .Add("merknad", string.Empty)))
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "basisregistrering"}, new XmlElementHelper()
                                        .Add("merknad", new XmlElementHelper())))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("merknad", string.Empty)
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("merknad", new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 7");
            testResults.Should().Contain(r => r.Message.Equals("Antall merknader i mappe: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Antall merknader i saksmappe: 3"));
            testResults.Should().Contain(r => r.Message.Equals("Antall merknader i basisregistrering: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Antall merknader i journalpost: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Antall merknader i dokumentbeskrivelse: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void CommentsAreFoundInDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("merknad", new XmlElementHelper())))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("merknad", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 1");
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Antall merknader i saksmappe: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 1");
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Antall merknader i saksmappe: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void NoCommentsAreFound()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2"))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
