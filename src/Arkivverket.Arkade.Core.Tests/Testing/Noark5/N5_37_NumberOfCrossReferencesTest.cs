using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_37_NumberOfCrossReferencesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReturnNumberOfCrossReferences()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to class
                                    .Add("referanseTilKlasse", "some-reference-identifier"))
                                .Add("klasse", new XmlElementHelper() // Nested class
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to class
                                        .Add("referanseTilKlasse", "some-reference-identifier")))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to folder
                                        .Add("referanseTilMappe", "some-reference-identifier"))
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to registration
                                        .Add("referanseTilRegistrering", "some-reference-identifier")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_37_NumberOfCrossReferences());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 4");
            testResults.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra klasser: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra mapper: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra basisregistreringer: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void ShouldReturnNumberOfCrossReferencesInEachArchivepart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to class
                                    .Add("referanseTilKlasse", "some-reference-identifier"))
                                .Add("klasse", new XmlElementHelper() // Nested class
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to class
                                        .Add("referanseTilKlasse", "some-reference-identifier")))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to folder
                                        .Add("referanseTilMappe", "some-reference-identifier"))
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to registration
                                        .Add("referanseTilRegistrering", "some-reference-identifier"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to registration
                                    .Add("referanseTilRegistrering", "some-reference-identifier"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_37_NumberOfCrossReferences());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 5");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 4");
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra klasser: 2"));
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra mapper: 1"));
            arkivdel1Results?.Should()
                .Contain(r => r.Message.Equals("Antall kryssreferanser fra basisregistreringer: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 1");
            arkivdel2Results?.Should()
                .Contain(r => r.Message.Equals("Antall kryssreferanser fra basisregistreringer: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(7);
        }

        [Fact]
        public void NumberOfCrossReferencesIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_37_NumberOfCrossReferences());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
