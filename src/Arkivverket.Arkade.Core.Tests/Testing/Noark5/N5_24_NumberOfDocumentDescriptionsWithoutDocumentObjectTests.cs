using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
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
                                    .Add("registreringsID", "regId1")
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("systemID", "dokBesSysId1")
                                        .Add("dokumentnummer", "1"))
                                    .Add("dokumentbeskrivelse", new XmlElementHelper()
                                        .Add("systemID", "dokBesSysId2")
                                        .Add("dokumentnummer", "2")))))))
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

            testRun.TestResults.TestResultSets[0].TestsResults[0].Message.Should().Be("Totalt: 2");
            testRun.TestResults.TestResultSets[0].TestsResults[1].Message.Should()
                .Be("Dokumentbeskrivelse (systemID, registreringsID, dokumentnummer): dokBesSysId1, regId1, 1");
            testRun.TestResults.TestResultSets[0].TestsResults[2].Message.Should()
                .Be("Dokumentbeskrivelse (systemID, registreringsID, dokumentnummer): dokBesSysId2, regId1, 2");

            testRun.TestResults.TestResultSets[1].TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Theory]
        [InlineData("utgår", "avsluttet", "arkivert")]
        [InlineData("avsluttet", "utgår", "arkivert")]
        [InlineData("avsluttet", "avsluttet", "utgår")]
        public void ShouldNotReportMissingDocumentObjectIfStatusUtgaar(string superFolderStatus, string folderStatus, string regStatus)
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper())
                                            .Add("journalstatus", regStatus))
                                        .Add("saksstatus", folderStatus))
                                    .Add("saksstatus", superFolderStatus))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject());

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldReportAmountOfMissingDocumentObjectsForPhysicalStorage()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("dokumentmedium", "fysisk arkiv"))))
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("dokumentmedium", "fysisk medium")))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject());

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");
            testRun.TestResults.TestsResults[1].Message.Should()
                .Be(string.Format(Noark5Messages.DocumentDescriptionsWithoutDocumentObjectsAndPhysicalStorage, 2));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }
    }
}