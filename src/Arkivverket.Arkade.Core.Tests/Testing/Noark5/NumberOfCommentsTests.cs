using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfCommentsTests
    {
        [Fact]
        public void NumberOfCommentsInFoldersIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("klasse",
                                        new XmlElementHelper().Add("klasse",
                                            new XmlElementHelper().Add("mappe",
                                                new XmlElementHelper().Add("merknad",
                                                    new XmlElementHelper().Add("merknadstekst", "enMerknad")
                                                ))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfComments());

            testRun.Results.Should().Contain(r => r.Message.Equals("Antall merknader i mapper: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall merknader i basisregistreringer: 0"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall merknader i dokumentbeskrivelser: 0"));
        }

        [Fact]
        public void NumberOfCommentsInFolderIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someSystemId_1")
                            .Add("klassifikasjonssystem",
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

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i mapper: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i basisregistreringer: 0"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i dokumentbeskrivelser: 0"));
        }

        [Fact]
        public void NumberOfCommentsIsMoreThenOneInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()
                                        .Add("mappe", new XmlElementHelper()
                                            .Add("merknad", new XmlElementHelper()))
                                        .Add("mappe", new XmlElementHelper()
                                            .Add("registrering", new[] {"xsi:type", "basisregistrering"}, new XmlElementHelper()
                                                    .Add("merknad", new XmlElementHelper()))
                                            .Add("registrering", new XmlElementHelper()
                                                .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                    .Add("merknad", new XmlElementHelper())))
                                        ))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "basisregistrering"}, new XmlElementHelper()
                                        .Add("merknad", new XmlElementHelper()
                                        )))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfComments());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_1 - Antall merknader i mapper: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_1 - Antall merknader i basisregistreringer: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_1 - Antall merknader i dokumentbeskrivelser: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID) someSystemId_2 - Antall merknader i basisregistreringer: 1"));
         }
    }
}


// Forekomster av elementet merknad i mappe, registreringstypen basisregistrering og dokumentbeskrivelse telles opp.