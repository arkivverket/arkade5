using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_11_NumberOfFoldersPerYearTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReturnNumberOfFoldersForSingleYear()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                            .Add("systemID", "someSystemId_1")
                            .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_11_NumberOfFoldersPerYear());

            testRun.TestResults.TestsResults[0].Message.Should().Contain("1863: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldReturnNumberOfFoldersForSeveralYears()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someSystemId_1").Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z")))))
                    .Add("arkivdel",
                        new XmlElementHelper().Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("opprettetDato", "1865-10-18T00:00:00Z"))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("opprettetDato", "1864-10-18T00:00:00Z")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_11_NumberOfFoldersPerYear());

            testRun.TestResults.TestResultSets[0].TestsResults[0].Message.Should().Contain("1863: 2");
            testRun.TestResults.TestResultSets[1].TestsResults[0].Message.Should().Contain("1864: 1");
            testRun.TestResults.TestResultSets[1].TestsResults[1].Message.Should().Contain("1865: 1");

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldReturnNumberOfFoldersForNoYears()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                            .Add("systemID", "someSystemId_1") 
                            .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("title", "Some title"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_11_NumberOfFoldersPerYear());

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void ShouldReturnTwoFoldersForSeveralYearsInTwoDifferentArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z"))
                                .Add("mappe", new XmlElementHelper().Add("opprettetDato", "1863-10-18T00:00:00Z")))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper().Add("opprettetDato", "1865-10-18T00:00:00Z"))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("opprettetDato", "1864-10-18T00:00:00Z")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_11_NumberOfFoldersPerYear());

            testRun.TestResults.TestResultSets[0].TestsResults.Should().Contain(r => r.Message.Equals("1863: 2"));
            testRun.TestResults.TestResultSets[1].TestsResults.Should().Contain(r => r.Message.Equals("1864: 1"));
            testRun.TestResults.TestResultSets[1].TestsResults.Should().Contain(r => r.Message.Equals("1865: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }
    }
}
