using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_18_NumberOfRegistrationsPerYearTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReturnNumberOfRegistrationsForSingleYear()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("registrering",
                                        new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_18_NumberOfRegistrationsPerYear());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("1863: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldReturnNumberOfRegistrationsSeveralYears()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper().Add("klasse",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "1865-10-18T00:00:00Z")))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato",
                                                "1864-10-18T00:00:00Z"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_18_NumberOfRegistrationsPerYear());

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("1863: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("1864: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("1865: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldReturnNumberOfRegistrationsForNoYears()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper().Add("klasse",
                            new XmlElementHelper().Add("mappe",
                                new XmlElementHelper().Add("title", "Some title"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_18_NumberOfRegistrationsPerYear());

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void ShouldReturnNumberOfRegistrationsForSeveralYearsInTwoDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z")))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper().Add("klasse",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato", "1865-10-18T00:00:00Z")))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("opprettetDato",
                                                "1864-10-18T00:00:00Z"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_18_NumberOfRegistrationsPerYear());

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("1863: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("1864: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("1865: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }
    }
}
