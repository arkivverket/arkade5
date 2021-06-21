using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_13_NumberOfFoldersPerClassTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindFoldersForSomeClassesOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse", // Has 2 folders
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_1")
                                        .Add("mappe", string.Empty)
                                        .Add("mappe", string.Empty))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_2")
                                        .Add("klasse", // Has 1 folder
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_3")
                                                .Add("mappe", string.Empty)))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_4")
                                        .Add("klasse", // Has no folders
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_5"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_13_NumberOfFoldersPerClass());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_1 - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_3 - Antall: 1"));
            testResults.Should().Contain(r => 
                r.Message.Equals("Klasser uten mapper (og uten underklasser) - Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldFindFoldersForSomeClassesOnDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("tittel", "someTitle_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has 2 folders
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_1")
                                            .Add("mappe", string.Empty)
                                            .Add("mappe", string.Empty))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_2")
                                            .Add("klasse", // Has 1 folder
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_3")
                                                    .Add("mappe", string.Empty)))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_4")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_5")))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has 2 folders
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_6")
                                            .Add("mappe", string.Empty)
                                            .Add("mappe", string.Empty))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_7")
                                            .Add("klasse", // Has 1 folder
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_8")
                                                    .Add("mappe", string.Empty)))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_9")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_10"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_13_NumberOfFoldersPerClass());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Klasser uten mapper (og uten underklasser) - Antall: 2"
            ));

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_1 - Antall: 2"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_3 - Antall: 1"));
            arkivdel1Results.Should().Contain(r =>
                r.Message.Equals("Klasser uten mapper (og uten underklasser) - Antall: 1"));


            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_6 - Antall: 2"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Klasse (systemID) someClassSystemId_8 - Antall: 1"));
            arkivdel2Results.Should().Contain(r =>
                r.Message.Equals("Klasser uten mapper (og uten underklasser) - Antall: 1"));


            testRun.TestResults.GetNumberOfResults().Should().Be(7);
        }

        [Fact]
        public void ShouldNotFindAnyFoldersForAnyClassesOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse", // Has no folders
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_1"))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_2")
                                        .Add("klasse", // Has no folders
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_3")))
                                .Add("klasse", // Has sub-class
                                    new XmlElementHelper()
                                        .Add("systemID", "someClassSystemId_4")
                                        .Add("klasse", // Has no folders
                                            new XmlElementHelper()
                                                .Add("systemID", "someClassSystemId_5"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_13_NumberOfFoldersPerClass());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Klasser uten mapper (og uten underklasser) - Antall: 3"
            ));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldNotFindAnyFoldersForAnyClassesInAnyArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("tittel", "someTitle_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has no folders
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_1"))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_2")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_3")))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_4")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_5")))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper()
                                    .Add("klasse", // Has no folders
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_6"))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_7")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_8")))
                                    .Add("klasse", // Has sub-class
                                        new XmlElementHelper()
                                            .Add("systemID", "someClassSystemId_9")
                                            .Add("klasse", // Has no folders
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_10"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_13_NumberOfFoldersPerClass());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals(
                "Klasser uten mapper (og uten underklasser) - Antall: 6"
            ));

            TestResultSet arkivdel1 = testRun.TestResults.TestResultSets[0];
            arkivdel1.TestsResults.Should().Contain(r =>
                r.Message.Equals("Klasser uten mapper (og uten underklasser) - Antall: 3"));

            TestResultSet arkivdel2 = testRun.TestResults.TestResultSets[1];
            arkivdel2.TestsResults.Should().Contain(r =>
                r.Message.Equals("Klasser uten mapper (og uten underklasser) - Antall: 3"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }
    }
}
