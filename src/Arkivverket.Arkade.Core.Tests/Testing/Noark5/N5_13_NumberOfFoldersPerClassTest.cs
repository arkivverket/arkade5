using Arkivverket.Arkade.Core.Base;
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasse (systemID): someClassSystemId_1 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasse (systemID): someClassSystemId_3 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasser uten mapper (og uten underklasser) - Antall: 1"
            ));
            testRun.Results.Count.Should().Be(3);
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someTitle_1 - Klasse (systemID): someClassSystemId_1 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someTitle_1 - Klasse (systemID): someClassSystemId_3 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someTitle_2 - Klasse (systemID): someClassSystemId_6 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someTitle_2 - Klasse (systemID): someClassSystemId_8 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasser uten mapper (og uten underklasser) - Antall: 2"
            ));
            testRun.Results.Count.Should().Be(5);
        }
    }
}
