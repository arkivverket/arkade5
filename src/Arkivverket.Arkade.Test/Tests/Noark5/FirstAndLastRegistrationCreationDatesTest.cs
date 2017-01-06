using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class FirstAndLastRegistrationCreationDatesTest
    {
        [Fact]
        public void ShouldFindValidAndInvalidCreationDates()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            // Invalid format:
                                            new XmlElementHelper().Add("opprettetDato", " -- invalid format -- ")))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            // First (valid) date:
                                            new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))))))
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "1864-10-18T00:00:00Z")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                // Last (valid) date:
                                                new XmlElementHelper().Add("opprettetDato", "1865-10-18T00:00:00Z"))))))));

            TestRun testRun = helper.RunEventsOnTest(new FirstAndLastRegistrationCreationDates());

            testRun.Results.Should().Contain(r => r.Message.Equals("Antall registreringer funnet: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Første registrering: Opprettet 18.10.1863"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Siste registrering: Opprettet 18.10.1865"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 1"));
            testRun.Results.Count.Should().Be(4);
        }

        [Fact]
        public void ShouldFindNoValidRegistrationCreationDates()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "lorem ipsum")))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "123456789"))))))
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "1864")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "10 18"))))))));

            TestRun testRun = helper.RunEventsOnTest(new FirstAndLastRegistrationCreationDates());

            testRun.Results.Should().Contain(r => r.Message.Equals("Antall registreringer funnet: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Ingen gyldige datoer for registreringsopprettelse funnet"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 4"));
            testRun.Results.Count.Should().Be(3);
        }
    }
}
