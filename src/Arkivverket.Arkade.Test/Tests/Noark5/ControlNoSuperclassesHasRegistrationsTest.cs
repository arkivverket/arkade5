using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ControlNoSuperclassesHasRegistrationsTest
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

            TestRun testRun = helper.RunEventsOnTest(new ControlNoSuperclassesHasRegistrations());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasse med systemID someClassSystemId_2"
            ));
            testRun.Results.Count.Should().Be(1);
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

            TestRun testRun = helper.RunEventsOnTest(new ControlNoSuperclassesHasRegistrations());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Klasse med systemID someClassSystemId_2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Klasse med systemID someClassSystemId_5"
            ));
            testRun.Results.Count.Should().Be(2);
        }
    }
}
