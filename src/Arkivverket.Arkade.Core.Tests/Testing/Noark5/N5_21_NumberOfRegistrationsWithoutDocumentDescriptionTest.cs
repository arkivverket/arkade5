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
                                            .Add("someSubElement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("someSubElement", "some value"))))))));

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
                                            .Add("someSubElement", "some value"))))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "regSysId1")
                                        .Add("registreringsID", "regId1")
                                        .Add("someSubElement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 1"));

            testRun.TestResults.TestsResults[1].Message.Should()
                .Be("Registrering (systemID, registreringsID): regSysId1, regId1");

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void ShouldFindTwoRegistrationsWithoutDocumentDescriptionInOneOfTwoArchiveParts()
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
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "regSysId1")
                                        .Add("registreringsID", "regId1"))
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "regSysId2")
                                        .Add("registreringsID", "regId2")))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));

            testRun.TestResults.TestResultSets[0].TestsResults[0].Message.Should()
                .Be("Totalt: 0");

            testRun.TestResults.TestResultSets[1].TestsResults[0].Message.Should()
                .Be("Totalt: 2");
            testRun.TestResults.TestResultSets[1].TestsResults[1].Message.Should()
                .Be("Registrering (systemID, registreringsID): regSysId1, regId1");
            testRun.TestResults.TestResultSets[1].TestsResults[2].Message.Should()
                .Be("Registrering (systemID, registreringsID): regSysId2, regId2");

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Theory]
        [InlineData("utgår", "avsluttet", "arkivert")]
        [InlineData("avsluttet", "utgår", "arkivert")]
        [InlineData("avsluttet", "avsluttet", "utgår")]
        public void ShouldNotReportRegistrationIfStatusUtgaar(string superFolderStatus, string folderStatus, string regStatus)
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
                                            .Add("journalstatus", regStatus))
                                        .Add("saksstatus", folderStatus))
                                    .Add("saksstatus", superFolderStatus))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_21_NumberOfRegistrationsWithoutDocumentDescription());

            testRun.TestResults.TestsResults[0].Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}