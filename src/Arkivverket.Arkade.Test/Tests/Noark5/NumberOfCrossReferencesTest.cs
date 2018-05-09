using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfCrossReferencesTest
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

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCrossReferences());

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
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("kryssreferanse", new XmlElementHelper()
                                    // Reference to registration
                                    .Add("referanseTilRegistrering", "some-reference-identifier"))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCrossReferences());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID): someSystemId_1 - Antall kryssreferanser fra klasser: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID): someSystemId_1 - Antall kryssreferanser fra mapper: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID): someSystemId_1 - Antall kryssreferanser fra basisregistreringer: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals(
                    "Arkivdel (systemID): someSystemId_2 - Antall kryssreferanser fra basisregistreringer: 1"));
        }
    }
}
