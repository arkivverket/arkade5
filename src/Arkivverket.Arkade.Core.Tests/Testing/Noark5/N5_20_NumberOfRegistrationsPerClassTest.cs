using Arkivverket.Arkade.Core.Base;
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasse (systemID): someClassSystemId_1 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasse (systemID): someClassSystemId_3 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasser uten registreringer (og uten underklasser) - Antall: 1"
            ));
            testRun.Results.Count.Should().Be(3);
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Klasse (systemID): someClassSystemId_1 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Klasse (systemID): someClassSystemId_3 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Klasse (systemID): someClassSystemId_6 - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Klasse (systemID): someClassSystemId_8 - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Klasser uten registreringer (og uten underklasser) - Antall: 2"
            ));
            testRun.Results.Count.Should().Be(5);
        }
    }
}
