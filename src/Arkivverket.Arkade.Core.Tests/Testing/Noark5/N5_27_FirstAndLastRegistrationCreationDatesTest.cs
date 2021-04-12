using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_27_FirstAndLastRegistrationCreationDatesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindValidAndInvalidCreationDatesInDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper() // Invalid format:
                                        .Add("opprettetDato", " -- invalid format -- "))
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("opprettetDato", "1863-10-18T00:00:00Z")))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("opprettetDato", "1864-10-18T00:00:00Z")))
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("opprettetDato", "1865-10-18+01:00"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_27_FirstAndLastRegistrationCreationDates());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 4"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Siste registrering: Opprettet 18.10.1863"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Første registrering: Opprettet 18.10.1864"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Siste registrering: Opprettet 18.10.1865"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 1"));
            testRun.Results.Count.Should().Be(6);
        }

        [Fact]
        public void ShouldFindNoValidRegistrationCreationDates()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("systemID", "someSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "lorem ipsum")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "123456789"))))))
                    .Add("arkivdel",
                        new XmlElementHelper().Add("systemID", "someSystemId_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper().Add("klasse",
                                        new XmlElementHelper()
                                            .Add("mappe",
                                                new XmlElementHelper().Add("registrering",
                                                    new XmlElementHelper().Add("opprettetDato", "1864-10-09+1:00")))
                                            .Add("mappe",
                                                new XmlElementHelper().Add("registrering",
                                                    new XmlElementHelper().Add("opprettetDato", "10 18"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_27_FirstAndLastRegistrationCreationDates());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 4"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 4"));
            testRun.Results.Count.Should().Be(2);
        }
    }
}
