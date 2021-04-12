using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_21_NumberOfRegistrationsWithoutDocumentDescriptionTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindNoRegistrationsWithoutDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.Results[0].Message.Should().Be("Totalt: 0");
        }

        [Fact]
        public void ShouldFindOneRegistrationsWithoutDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("somesubelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.Results[0].Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void ShouldFindTwoRegistrationsWithoutDocumentDescriptionInOneOfTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", string.Empty))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", string.Empty)
                                    .Add("registrering", string.Empty))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_2 - someTitle_2: 2"));
        }
    }
}