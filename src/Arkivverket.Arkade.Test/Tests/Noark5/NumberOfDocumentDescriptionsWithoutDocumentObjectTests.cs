using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDocumentDescriptionsWithoutDocumentObjectTests
    {
        [Fact]
        public void ShouldReturnDocumentDescriptionsWithoutWithoutDocumentObject()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering", new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt", new XmlElementHelper().Add("versjonsnummer", "1") ))))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse", new XmlElementHelper().Add("systemID", "journpost56fd39c30a5373.09722056") )))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDocumentDescriptionsWithoutDocumentObject());

            testRun.Results[0].Message.Should().Be("1");
        }
    }
}

