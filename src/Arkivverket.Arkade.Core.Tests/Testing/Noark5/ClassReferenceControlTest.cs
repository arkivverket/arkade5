using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class ClassReferenceControlTest
    {
        [Fact]
        public void ReferencesAreValid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse",
                                    new XmlElementHelper().Add("systemID", "someClassSystemId"))
                                .Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe", new[] { "xsi:type", "saksmappe" },
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_1")
                                                .Add("referanseSekundaerKlassifikasjon", "someClassSystemId"))
                                        .Add("mappe", new[] { "xsi:type", "saksmappe" },
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_2")
                                                .Add("referanseSekundaerKlassifikasjon", "someClassSystemId"))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_51_ClassReferenceControl());

            testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void SomeReferencesAreInvalid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse",
                                    new XmlElementHelper().Add("systemID", "someClassSystemId"))
                                .Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe", new[] { "xsi:type", "saksmappe" }, // Has invalid reference
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_1")
                                                .Add("referanseSekundaerKlassifikasjon", "someNonExistingClassSystemId"))
                                        .Add("mappe", new[] { "xsi:type", "saksmappe" }, // Has invalid reference
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_2")
                                                .Add("referanseSekundaerKlassifikasjon", "someNonExistingClassSystemId"))
                                        .Add("mappe", // Not of type "saksmappe" - is skipped
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_3")
                                                .Add("referanseSekundaerKlassifikasjon", "someNonExistingClassSystemId"))
                                        .Add("mappe", new[] { "xsi:type", "saksmappe" }, // No reference - is skipped
                                            new XmlElementHelper()
                                                .Add("systemID", "someFolderSystemId_4"))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_51_ClassReferenceControl());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Referanse fra mappe (systemID) someFolderSystemId_1 til klasse (systemID) someNonExistingClassSystemId er ikke gyldig"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Referanse fra mappe (systemID) someFolderSystemId_2 til klasse (systemID) someNonExistingClassSystemId er ikke gyldig"
            ));
            testRun.Results.Count.Should().Be(2);
        }
    }
}
