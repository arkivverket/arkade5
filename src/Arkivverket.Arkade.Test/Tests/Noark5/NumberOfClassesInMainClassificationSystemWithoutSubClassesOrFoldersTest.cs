using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
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
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", string.Empty))
                            .Add("klasse", new XmlElementHelper()
                                .Add("registrering", string.Empty))
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", string.Empty))))));

            TestRun testRun =
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.First().Message.Should().Be("0");
        }

        [Fact]
        public void NumberOfEmptyClassesIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", string.Empty))
                            .Add("klasse", string.Empty)))
                );

            TestRun testRun =
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.First().Message.Should().Be("2");
        }

        [Fact]
        public void NumberOfEmptyClassesIsTwoInTwoDifferentArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", string.Empty)))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someArchivePartSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", string.Empty))));

            TestRun testRun =
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results[0].Message.Should().Be("Arkivdel (systemID) someArchivePartSystemId_1: 1");
            testRun.Results[1].Message.Should().Be("Arkivdel (systemID) someArchivePartSystemId_2: 1");
        }

        [Fact]
        public void NumberOfEmptyClassesIsOnlyCountedInPrimaryClassificationSystem()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper())
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse", string.Empty))));

            TestRun testRun =
                helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations());

            testRun.Results.First().Message.Should().Be("0");
        }
    }
}
