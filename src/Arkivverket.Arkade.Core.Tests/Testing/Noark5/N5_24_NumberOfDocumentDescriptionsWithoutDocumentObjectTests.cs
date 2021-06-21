using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_24_NumberOfDocumentDescriptionsWithoutDocumentObjectTests : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindNoDocumentDescriptionWithoutDocumentObject()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_1")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()))))
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldFindTwoDocumentDescriptionsWithoutDocumentObjectInOneOfTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv", new XmlElementHelper()
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_1")
                    .Add("tittel", "someTitle_1")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper())
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()))))))
                .Add("arkivdel", new XmlElementHelper()
                    .Add("systemID", "someSystemId_2")
                    .Add("tittel", "someTitle_2")
                    .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                            .Add("mappe", new XmlElementHelper()
                                .Add("registrering", new XmlElementHelper()
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("dokumentobjekt", new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testResults.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1: 2"));
            testResults.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2: 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }
    }
}