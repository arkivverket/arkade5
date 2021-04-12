using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_37_NumberOfCrossReferencesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReturnNumberOfCrossReferences()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to class
                                    .Add("referanseTilKlasse", "some-reference-identifier"))
                                .Add("klasse", new XmlElementHelper() // Nested class
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to class
                                        .Add("referanseTilKlasse", "some-reference-identifier")))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to folder
                                        .Add("referanseTilMappe", "some-reference-identifier"))
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to registration
                                        .Add("referanseTilRegistrering", "some-reference-identifier")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_37_NumberOfCrossReferences());

            testRun.Results.First().Message.Should().Be("Totalt: 4");
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra klasser: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall kryssreferanser fra mapper: 1"));
            testRun.Results.Should()
                .Contain(r => r.Message.Equals("Antall kryssreferanser fra basisregistreringer: 1"));
        }

        [Fact]
        public void ShouldReturnNumberOfCrossReferencesInEachArchivepart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to class
                                    .Add("referanseTilKlasse", "some-reference-identifier"))
                                .Add("klasse", new XmlElementHelper() // Nested class
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to class
                                        .Add("referanseTilKlasse", "some-reference-identifier")))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to folder
                                        .Add("referanseTilMappe", "some-reference-identifier"))
                                    .Add("kryssreferanse", new XmlElementHelper()
                                        // Reference to registration
                                        .Add("referanseTilRegistrering", "some-reference-identifier"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to registration
                                    .Add("referanseTilRegistrering", "some-reference-identifier"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_37_NumberOfCrossReferences());


            testRun.Results.First().Message.Should().Be("Totalt: 5");
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Antall kryssreferanser fra klasser: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Antall kryssreferanser fra mapper: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Antall kryssreferanser fra basisregistreringer: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Antall kryssreferanser fra basisregistreringer: 1"));
        }
    }
}
