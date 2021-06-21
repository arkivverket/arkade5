using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_21_NumberOfRegistrationsWithoutDocumentDescriptionTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindNoRegistrationsWithoutDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someSystemTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldFindOneRegistrationsWithoutDocumentDescription()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someSystemTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("somesubelement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("somesubelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldFindTwoRegistrationsWithoutDocumentDescriptionInOneOfTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", string.Empty))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", string.Empty)
                                    .Add("registrering", string.Empty))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1: 0"));
            testResults.Should().Contain(r => r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }
    }
}