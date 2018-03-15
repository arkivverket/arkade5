using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDocumentObjectsTests
    {
        [Fact]
        public void ShouldFindTwoDocumentObjects()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "1")))))
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "2")))))
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("systemID", "journpost56fd39c30a5373.09722056"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDocumentObjects());

            //testRun.Results.First().Message.Should().Be("Antall dokumentobjekter: 2");
            testRun.Results[0].Message.Should().Be("2");
        }


        [Fact]
        public void ShouldFindOneDocumentObjectPerArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_1")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "1"))))))))
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_2")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "2"))))))))
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDocumentObjects());

            testRun.Results.Should().Contain(r => r.Message.Equals("2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_2: 1"));
        }
    }
}