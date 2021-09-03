using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_12_ControlNoSuperclassesHasFoldersTest : LanguageDependentTest
    {
        [Fact]
        public void SomeClassesHasBothSubclassesAndFolders()
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
                                                    .Add("klasse", // Class has folder and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_2")
                                                            .Add("mappe", string.Empty)
                                                            .Add("klasse", // Class has folder only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_3")
                                                                    .Add("mappe", string.Empty)))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_12_ControlNoSuperclassesHasFolders());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 1");
            testRun.TestResults.TestsResults[1].Message.Should()
                .Be("Klasse med systemID someClassSystemId_2");
       
            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void SomeClassesHasBothSubclassesAndFoldersInSeveralArchiveParts()
        {
            XmlElementHelper helper =
                new XmlElementHelper()
                    .Add("arkiv",
                        new XmlElementHelper()
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_1")
                                    .Add("tittel", "someTitle_1")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_1")
                                                    .Add("klasse", // Class has folder and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_2")
                                                            .Add("mappe", string.Empty)
                                                            .Add("klasse", // Class has folder only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_3")
                                                                    .Add("mappe", string.Empty))))))
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_2")
                                    .Add("tittel", "someTitle_2")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_4")
                                                    .Add("klasse", // Class has folder and class = not ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_5")
                                                            .Add("mappe", string.Empty)
                                                            .Add("klasse", // Class has folder only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_6")
                                                                    .Add("mappe", string.Empty)))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_12_ControlNoSuperclassesHasFolders());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            TestResultSet arkivdel1 = testRun.TestResults.TestResultSets[0];
            arkivdel1.TestsResults.First().Message.Should().Be("Antall: 1");
            arkivdel1.TestsResults.Should().Contain(r => r.Message.Equals("Klasse med systemID someClassSystemId_2"));

            TestResultSet arkivdel2 = testRun.TestResults.TestResultSets[1];
            arkivdel2.TestsResults.First().Message.Should().Be("Antall: 1");
            arkivdel2.TestsResults.Should().Contain(r => r.Message.Equals("Klasse med systemID someClassSystemId_5"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void NoClassesHasBothSubclassesAndFolders()
        {
            XmlElementHelper helper =
                new XmlElementHelper()
                    .Add("arkiv",
                        new XmlElementHelper()
                            .Add("arkivdel",
                                new XmlElementHelper()
                                    .Add("systemID", "someArchivePartSystemId_1")
                                    .Add("tittel", "someTitle_1")
                                    .Add("klassifikasjonssystem",
                                        new XmlElementHelper()
                                            .Add("klasse", // Class has class only = ok
                                                new XmlElementHelper()
                                                    .Add("systemID", "someClassSystemId_1")
                                                    .Add("klasse", // Class has folder = ok
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someClassSystemId_2")
                                                            .Add("klasse", // Class has folder only = ok
                                                                new XmlElementHelper()
                                                                    .Add("systemID", "someClassSystemId_3")
                                                                    .Add("mappe", string.Empty)))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_12_ControlNoSuperclassesHasFolders());

            testRun.TestResults.GetAllResults().Count.Should().Be(1);

            TestResult testResult = testRun.TestResults.TestsResults.First();
            testResult.Message.Should().Be("Totalt: 0");
            testResult.Result.Should().Be(ResultType.Success);
            
        }
    }
}
