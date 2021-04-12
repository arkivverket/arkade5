using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
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

            testRun.Results.First().Message.Should().Be("Totalt: 0");
            testRun.Results.Should().HaveCount(1); // Zero empty classes not reported
        }

        [Fact]
        public void NumberOfEmptyClassesInPrimaryClassificationSystemIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", string.Empty))
                                .Add("klasse", string.Empty)))));

            TestRun testRun =
                helper.RunEventsOnTest(new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
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

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel) someArchivePartSystemId_1, someTitle_1 - klassifikasjonssystem (systemID) klassSys_1: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel) someArchivePartSystemId_2, someTitle_2 - klassifikasjonssystem (systemID) klassSys_1: 1"));
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

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel) someArchivePartSystemId_1, someTitle_1 - klassifikasjonssystem (systemID) klassSys_1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel) someArchivePartSystemId_2, someTitle_2 - klassifikasjonssystem (systemID) klassSys_1: 2"));
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

            testRun.Results.First().Message.Should().Be("Totalt: 0");
        }
    }
}
