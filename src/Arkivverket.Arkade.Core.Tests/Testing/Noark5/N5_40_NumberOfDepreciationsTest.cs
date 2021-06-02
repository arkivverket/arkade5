using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_40_NumberOfDepreciationsTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfDepreciationsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per telefon"))
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per e-post"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_40_NumberOfDepreciations());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Besvart per telefon: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Besvart per e-post: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void NumberOfDepreciationsIsTwoInTwoDifferentArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
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
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("avskrivning", new XmlElementHelper()
                                            .Add("avskrivningsmaate", "Besvart per telefon"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_40_NumberOfDepreciations());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 3"));

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 2");
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Besvart per telefon: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Besvart per e-post: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 1");
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Besvart per telefon: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void NumberOfDepreciationsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_40_NumberOfDepreciations());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
