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
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("referanseAvskrivesAvJournalpost", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());

            testRun.Results.Should().Contain(r => r.Message.Equals("Antall journalposter som avskriver andre journalposter: 1"));
        }

        [Fact]
        public void NumberOfDepreciationsIsOneInOneOfTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("referanseAvskrivesAvJournalpost", new XmlElementHelper()))
                                    ))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDepreciations());

            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_2 - Antall journalposter som avskriver andre journalposter: 1"));
        }
    }
}
