using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_20_NumberOfRegistrationsPerClassTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindRegistrationsForSomeClassesOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse", // Has 2 registrations
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_1")
                                        .Add("registrering", string.Empty)
                                        .Add("registrering", string.Empty))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_2")
                                        .Add("klasse", // Has 1 registration
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_3")
                                                .Add("registrering", string.Empty)))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_4")
                                        .Add("klasse", // Has no registrations
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_5"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_20_NumberOfRegistrationsPerClass());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_1: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_3: 1"));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Klasser uten registreringer (og uten underklasser) - Antall: 1"
            ));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldFindRegistrationsForSomeClassesOnDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("tittel", "someArchivePartTitle_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has 2 registrations
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_1")
                                            .Add("registrering", string.Empty)
                                            .Add("registrering", string.Empty))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_2")
                                            .Add("klasse", // Has 1 registration
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_3")
                                                    .Add("registrering", string.Empty)))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_4")
                                            .Add("klasse", // Has no registrations
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_5")))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has 2 registrations
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_6")
                                            .Add("registrering", string.Empty)
                                            .Add("registrering", string.Empty))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_7")
                                            .Add("klasse", // Has 1 registration
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_8")
                                                    .Add("registrering", string.Empty)))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_9")
                                            .Add("klasse", // Has no registrations
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_10"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_20_NumberOfRegistrationsPerClass());


            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_1: 2"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_3: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_6: 2"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klasse (SystemID) someClassSystemId_8: 1"));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Klasser uten registreringer (og uten underklasser) - Antall: 2"
            ));
            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }
    }
}
