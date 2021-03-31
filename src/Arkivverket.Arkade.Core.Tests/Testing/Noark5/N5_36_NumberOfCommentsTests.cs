using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_36_NumberOfCommentsTests : LanguageDependentTest
    {


        [Fact]
        public void CommentsAreFoundUnderCaseFolderBaseRegistrationJournalPostAndDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                    .Add("merknad", string.Empty)
                                    .Add("merknad", string.Empty))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("merknad", string.Empty)))
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] {"xsi:type", "saksmappe"}, new XmlElementHelper()
                                    .Add("merknad", string.Empty)))
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "basisregistrering"}, new XmlElementHelper()
                                        .Add("merknad", new XmlElementHelper())))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("merknad", string.Empty)
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("merknad", new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            testRun.Results.First().Message.Should().Be("Totalt: 7");

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i mappe: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i saksmappe: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i basisregistrering: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i journalpost: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall merknader i dokumentbeskrivelse: 1"));
        }

        [Fact]
        public void CommentsAreFoundInDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("merknad", new XmlElementHelper())))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new[] { "xsi:type", "saksmappe" }, new XmlElementHelper()
                                    .Add("merknad", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Antall merknader i saksmappe: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Antall merknader i saksmappe: 1"));
        }

        [Fact]
        public void NoCommentsAreFound()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2"))));

            TestRun testRun = helper.RunEventsOnTest(new N5_36_NumberOfComments());

            testRun.Results.First().Message.Should().Be("Totalt: 0");
        }
    }
}
