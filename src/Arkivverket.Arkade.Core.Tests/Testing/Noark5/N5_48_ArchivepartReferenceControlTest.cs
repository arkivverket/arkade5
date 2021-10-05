using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_48_ArchivepartReferenceControlTest : LanguageDependentTest
    {
        [Fact]
        public void ReferencesAreValid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("mappe",
                                        new XmlElementHelper()
                                            .Add("systemID", "someFolderSystemId")
                                            .Add("referanseArkivdel", "someArchivePartSystemId")
                                            .Add("registrering",
                                                new XmlElementHelper()
                                                    .Add("systemID", "someRegistrationSystemId")
                                                    .Add("referanseArkivdel", "someArchivePartSystemId")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someDocumentDescriptionSystemId")
                                                            .Add("referanseArkivdel", "someArchivePartSystemId")))
                                            .Add("registrering",
                                                new XmlElementHelper()
                                                    .Add("systemID", "someRegistrationSystemId")
                                                    .Add("referanseArkivdel", "someArchivePartSystemId")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someDocumentDescriptionSystemId")
                                                            .Add("referanseArkivdel", "someArchivePartSystemId"))))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_48_ArchivepartReferenceControl());

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void SomeReferencesAreInvalid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("mappe",
                                        new XmlElementHelper()
                                            .Add("systemID", "someFolderSystemId")
                                            .Add("referanseArkivdel", "someMissingArchivePartSystemId")
                                            .Add("registrering",
                                                new XmlElementHelper()
                                                    .Add("systemID", "someRegistrationSystemId")
                                                    .Add("referanseArkivdel", "someMissingArchivePartSystemId")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("systemID", "someDocumentDescriptionSystemId")
                                                            .Add("referanseArkivdel", "someMissingArchivePartSystemId"))))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_48_ArchivepartReferenceControl());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 3");
            testResults.Should().Contain(r => r.Message.Equals(
                "Referanse fra mappe (systemID) someFolderSystemId til arkivdel (systemID) someMissingArchivePartSystemId er ikke gyldig"
            ));
            testResults.Should().Contain(r => r.Message.Equals(
                "Referanse fra registrering (systemID) someRegistrationSystemId til arkivdel (systemID) someMissingArchivePartSystemId er ikke gyldig"
            ));
            testResults.Should().Contain(r => r.Message.Equals(
                "Referanse fra dokumentbeskrivelse (systemID) someDocumentDescriptionSystemId til arkivdel (systemID) someMissingArchivePartSystemId er ikke gyldig"
            ));
            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }
    }
}
