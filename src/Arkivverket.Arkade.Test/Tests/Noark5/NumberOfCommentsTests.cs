using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfCommentsTests
    {
        [Fact]
        public void NumberOfCommentsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("klasse",
                                        new XmlElementHelper().Add("mappe",
                                        new XmlElementHelper().Add("merknad", 
                                        new XmlElementHelper().Add("merknadstekst", "enMerknad")
                                        ))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfComments());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfCommentsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("klasse",
                                        new XmlElementHelper().Add("mappe",
                                        new XmlElementHelper().Add("merknad",
                                        new XmlElementHelper().Add("merknadstekst", "enMerknad")))
                                        .Add("mappe",
                                        new XmlElementHelper().Add("merknad",
                                        new XmlElementHelper().Add("merknadstekst", "mer merknad")))
                                        ))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfComments());

            testRun.Results.First().Message.Should().Contain("2");
        }
    }
}
