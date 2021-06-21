using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_43_NumberOfClassificationsTest : LanguageDependentTest
    {
        [Fact]
        public void HasSeverealClassificationsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_43_NumberOfClassifications());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 7");
            testResults.Should().Contain(r => r.Message.Equals("Graderinger i arkivdel - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Graderinger i klasse - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Graderinger i mappe - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Graderinger i registrering - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Graderinger i dokumentbeskrivelse - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void HasSeverealClassificationsOnSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someArchivePartTitle_1")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig"))))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someArchivePartTitle_2")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_43_NumberOfClassifications());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 14");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someArchivePartSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 7");
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Graderinger i arkivdel - Antall: 1"));
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Graderinger i klasse - Antall: 1"));
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Graderinger i mappe - Antall: 2"));
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Graderinger i registrering - Antall: 1"));
            arkivdel1Results?.Should().Contain(r => r.Message.Equals("Graderinger i dokumentbeskrivelse - Antall: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someArchivePartSystemId_2"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 7");
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Graderinger i arkivdel - Antall: 1"));
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Graderinger i klasse - Antall: 1"));
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Graderinger i mappe - Antall: 2"));
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Graderinger i registrering - Antall: 1"));
            arkivdel2Results?.Should().Contain(r => r.Message.Equals("Graderinger i dokumentbeskrivelse - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(13);
        }

        [Fact]
        public void HasNoClassificationsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper())))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper())))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_43_NumberOfClassifications());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
