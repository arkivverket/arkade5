using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_26_NumberOfDocumentObjectsTests : LanguageDependentTest
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

            TestRun testRun = helper.RunEventsOnTest(new N5_26_NumberOfDocumentObjects());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
        }


        [Fact]
        public void ShouldFindOneDocumentObjectPerArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_1")
                    .Add("tittel", "someTitle_1")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "1"))))))))
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_2")
                    .Add("tittel", "someTitle_2")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()
                                            .Add("versjonsnummer", "2"))))))))
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_26_NumberOfDocumentObjects());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_1 - someTitle_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_2 - someTitle_2: 1"));
        }
    }
}