using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_46_NumberOfConversionsTest : LanguageDependentTest
    {
        [Fact]
        public void HasConversionsInSingleArchivePart()
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
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()
                                                                                .Add("konvertering", string.Empty)))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()
                                                                                .Add("konvertering", string.Empty)))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_46_NumberOfConversions());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void HasConversionsInSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someArchivePartTitle_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()
                                                                                .Add("konvertering", string.Empty)))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()
                                                                                .Add("konvertering", string.Empty))))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someArchivePartTitle_2")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()
                                                                                .Add("konvertering", string.Empty)))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_46_NumberOfConversions());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 3");
            testResults.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1: 2"));
            testResults.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void HasNoConversions()
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
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("dokumentobjekt",
                                                                            new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_46_NumberOfConversions());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
