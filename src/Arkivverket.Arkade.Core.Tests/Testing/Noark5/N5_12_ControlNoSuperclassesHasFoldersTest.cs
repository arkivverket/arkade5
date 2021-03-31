using System.Linq;
using Arkivverket.Arkade.Core.Base;
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

            testRun.Results.First().Message.Should().Be("Totalt: 1");
            testRun.Results[1].Message.Should().Be(
                "Klasse med systemID someClassSystemId_2"
            );
       
            testRun.Results.Count.Should().Be(2);
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someTitle_1 - Klasse med systemID someClassSystemId_2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someTitle_2 - Klasse med systemID someClassSystemId_5"
            ));
            testRun.Results.Count.Should().Be(3);
        }
    }
}
