using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfDepreciationsTest
    {
        [Fact]
        public void NumberOfDepreciationsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per telefon"))
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per e-post"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Besvart per telefon: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Besvart per e-post: 1"));
        }

        [Fact]
        public void NumberOfDepreciationsIsTwoInTwoDifferentArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per telefon"))
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per e-post")))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per telefon"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());


            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 3"));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_1 - Besvart per telefon: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_1 - Besvart per e-post: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_2 - Besvart per telefon: 1"));
        }
    }
}
