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
                                        .Add("referanseTilMappe", "some-reference-identifier")))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCrossReferences());

            testRun.Results[0].Message.Should().Be("Referanser til klasse: 2");
            testRun.Results[1].Message.Should().Be("Referanser til mappe: 1");
        }
    }
}
