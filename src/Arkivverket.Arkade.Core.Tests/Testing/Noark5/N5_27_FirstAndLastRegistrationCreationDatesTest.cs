using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
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

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 4"));
            testRun.TestResults.TestsResults.Should().Contain(r =>
                r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 1"));

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Første registrering: Opprettet 18.10.1863"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Siste registrering: Opprettet 18.10.1863"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Første registrering: Opprettet 18.10.1864"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Siste registrering: Opprettet 18.10.1865"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
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

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 4"));
            testRun.TestResults.TestsResults.Should().Contain(r =>
                r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 4"));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void ShouldFindValidAndInvalidCreationDatesInSingleArchivePart()
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
                                            .Add("opprettetDato", "1863-10-18T00:00:00Z"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_27_FirstAndLastRegistrationCreationDates());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testResults.Should().Contain(r =>
                r.Message.Equals("Antall ugyldige datoer for registreringsopprettelse funnet: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Første registrering: Opprettet 18.10.1863"));
            testResults.Should().Contain(r => r.Message.Equals("Siste registrering: Opprettet 18.10.1863"));

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }
    }
}
