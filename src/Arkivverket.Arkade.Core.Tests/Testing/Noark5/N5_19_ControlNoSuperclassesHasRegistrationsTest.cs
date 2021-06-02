using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Arkivverket.Arkade.Core.Testing;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_19_ControlNoSuperclassesHasRegistrationsTest : LanguageDependentTest
    {
        [Fact]
        public void SomeClassesHasBothSubclassesAndRegistrations()
        {
            XmlElementHelper helper =
                new XmlElementHelper()
                    .Add("arkiv",
                        new XmlElementHelper()
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_1")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_1")
                                                    .Add("klasse", // Class has registration and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_2")
                                                            .Add("registrering", string.Empty)
                                                            .Add("klasse", // Class has registration only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_3")
                                                                    .Add("registrering", string.Empty)))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_19_ControlNoSuperclassesHasRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 1");

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Klasse med systemID someClassSystemId_2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void SomeClassesHasBothSubclassesAndRegistrationsInSeveralArchiveParts()
        {
            XmlElementHelper helper =
                new XmlElementHelper()
                    .Add("arkiv",
                        new XmlElementHelper()
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_1")
                                    .Add("tittel", "someArchivePartTitle_1")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_1")
                                                    .Add("klasse", // Class has registration and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_2")
                                                            .Add("registrering", string.Empty)
                                                            .Add("klasse", // Class has registration only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_3")
                                                                    .Add("registrering", string.Empty))))))
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_2")
                                    .Add("tittel", "someArchivePartTitle_2")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_4")
                                                    .Add("klasse", // Class has registration and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_5")
                                                            .Add("registrering", string.Empty)
                                                            .Add("klasse", // Class has registration only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_6")
                                                                    .Add("registrering", string.Empty)))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_19_ControlNoSuperclassesHasRegistrations());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klasse med systemID someClassSystemId_2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klasse med systemID someClassSystemId_5"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }
    }
}
