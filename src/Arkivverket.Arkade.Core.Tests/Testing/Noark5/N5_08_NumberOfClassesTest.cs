using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_08_NumberOfClassesTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfClassesIsTwoInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    // Arkivdel 1
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "arkivdelTittel1")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()))))
                    // Arkivdel 2
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "arkivdelTittel2")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper())))));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 12");

            TestResultSet arkivdel1 = testRun.TestResults.TestResultSets.Find(s => s.Name.Contains("someSystemId_1"));
            arkivdel1?.TestsResults.First().Message.Should().Be("Antall klasser: 6");

            List<TestResult> testResultsA1C1 =
                arkivdel1?.TestResultSets.Find(s => s.Name.Contains("klassSys_1"))?.TestsResults;
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));

            List<TestResult> testResultsA1C2 =
                arkivdel1?.TestResultSets.Find(s => s.Name.Contains("klassSys_2"))?.TestsResults;
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));


            TestResultSet arkivdel2 = testRun.TestResults.TestResultSets.Find(s => s.Name.Contains("someSystemId_2"));
            arkivdel2?.TestsResults.First().Message.Should().Be("Antall klasser: 6");

            List<TestResult> testResultsA2C1 = 
                arkivdel2?.TestResultSets.Find(s => s.Name.Contains("klassSys_1"))?.TestsResults;
            testResultsA2C1.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA2C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA2C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));

            List<TestResult> testResultsA2C2 = 
                arkivdel2?.TestResultSets.Find(s => s.Name.Contains("klassSys_2"))?.TestsResults;
            testResultsA2C2.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA2C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA2C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));
            
            testRun.TestResults.GetNumberOfResults().Should().Be(15);
        }

        [Fact]
        public void ArchiveIsWithoutClassificationSystem()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("registrering", string.Empty))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("mappe", string.Empty)));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NumberOfClassesIsTwoInOneOfTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    // Arkivdel 1
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "arkivdelTittel1")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()))))
                    // Arkivdel 2
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "arkivdelTittel2")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2"))));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 6");

            TestResultSet arkivdel1 = testRun.TestResults.TestResultSets.Find(s => s.Name.Contains("someSystemId_1"));
            arkivdel1?.TestsResults.First().Message.Should().Be("Antall klasser: 6");

            List<TestResult> testResultsA1C1 = arkivdel1?.TestResultSets.Find(s => s.Name.Contains("klassSys_1"))
                ?.TestsResults;
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA1C1.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));

            List<TestResult> testResultsA1C2 = arkivdel1?.TestResultSets.Find(s => s.Name.Contains("klassSys_2"))
                ?.TestsResults;
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            testResultsA1C2.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));


            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall klasser: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(9);
        }

        [Fact]
        public void NumberOfClassesIsTwoInOneArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    // Arkivdel 1
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "arkivdelTittel1")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper())))));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 6");

            List<TestResult> classificationSystem1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("klassSys_1"))?.TestsResults;
            classificationSystem1Results?.First().Message.Should().Be("Antall klasser: 3");
            classificationSystem1Results.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            classificationSystem1Results.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));

            List<TestResult> classificationSystem2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("klassSys_2"))?.TestsResults;
            classificationSystem2Results.Should().Contain(r => r.Message.Equals("Antall klasser: 3"));
            classificationSystem2Results.Should().Contain(r => r.Message.Equals("Klasser på nivå 1: 2"));
            classificationSystem2Results.Should().Contain(r => r.Message.Equals("Klasser på nivå 2: 1"));
            
            testRun.TestResults.GetNumberOfResults().Should().Be(7);
        }
    }
}
