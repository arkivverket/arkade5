using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfClassesInMainClassificationSystemWithoutSubClassesOrFoldersTest
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
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.Should().BeEmpty(); // Zero empty classes not reported
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
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.First().Message.Should().Be("2");
        }

        [Fact]
        public void NumberOfEmptyClassesInPrimaryClassificationSystemIsTwoInTwoDifferentArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()
                                .Add("klasse", string.Empty)))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someArchivePartSystemId_1 - klassifikasjonssystem (systemID) klassSys_1: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someArchivePartSystemId_2 - klassifikasjonssystem (systemID) klassSys_1: 1"));
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
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_1")
                                    .Add("klasse", string.Empty)
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("klasse", string.Empty)))
                                .Add("klassifikasjonssystem", new XmlElementHelper()
                                    .Add("systemID", "klassSys_2")
                                    .Add("klasse", string.Empty))));

            TestRun testRun =
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someArchivePartSystemId_1 - klassifikasjonssystem (systemID) klassSys_1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someArchivePartSystemId_2 - klassifikasjonssystem (systemID) klassSys_1: 2"));
        }
    }
}
