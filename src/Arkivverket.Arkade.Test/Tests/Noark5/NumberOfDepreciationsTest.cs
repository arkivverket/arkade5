using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDepreciationsTest
    {
        [Fact]
        public void NumberOfDepreciationsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());

            testRun.Results.First().Message.Should().Be("Antall avskrivninger: 1");
        }

        [Fact]
        public void NumberOfDepreciationsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                            // No depreciation
                                            .Add("somesubelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());

            testRun.Results.First().Message.Should().Be("Antall avskrivninger: 0");
        }
    }
}
