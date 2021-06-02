using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Arkivverket.Arkade.Core.Testing;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesOrFoldersTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfEmptyClassesIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", string.Empty))
                                .Add("klasse", new XmlElementHelper()
                                    .Add("registrering", string.Empty))
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()
                                        .Add("mappe", string.Empty)))))));

            TestRun testRun =
                helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");
            testRun.TestResults.GetNumberOfResults().Should().Be(1); // Zero empty classes not reported
        }

        [Fact]
        public void NumberOfEmptyClassesInPrimaryClassificationSystemIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", string.Empty))
                                .Add("klasse", string.Empty)))));

            TestRun testRun =
                helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void NumberOfEmptyClassesInPrimaryClassificationSystemIsTwoInTwoDifferentArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()
                                .Add("klasse", string.Empty)))));

            TestRun testRun = helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klassifikasjonssystem (systemID) klassSys_1: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klassifikasjonssystem (systemID) klassSys_1: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void NumberOfEmptyClassesIsOnlyCountedInPrimaryClassificationSystems()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someTitle_1")
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_1")
                                    .Add("klasse", string.Empty)
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("klasse", string.Empty)))
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_2")
                                    .Add("klasse", string.Empty)))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someTitle_2")
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_1")
                                    .Add("klasse", string.Empty)
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("klasse", string.Empty)))
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_2")
                                    .Add("klasse", string.Empty))));

            TestRun testRun =
                helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 4");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klassifikasjonssystem (systemID) klassSys_1: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klassifikasjonssystem (systemID) klassSys_1: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
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

            TestRun testRun = helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");
        }
    }
}
