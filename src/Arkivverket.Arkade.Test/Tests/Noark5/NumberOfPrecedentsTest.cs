using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfPrecedentsTest
    {
        [Fact]
        public void NumberOfPrecedentsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("presedens", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfPrecedents());

            testRun.Results.First().Message.Should().Be("Antall presedenser: 1");
        }

        [Fact]
        public void NumberOfPrecedentsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                            // No precedent
                                            .Add("somesubelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfPrecedents());

            testRun.Results.First().Message.Should().Be("Antall presedenser: 0");
        }
    }
}
