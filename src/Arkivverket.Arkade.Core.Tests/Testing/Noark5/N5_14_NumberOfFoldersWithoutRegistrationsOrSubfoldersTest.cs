using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Testing;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_14_NumberOfFoldersWithoutRegistrationsOrSubfoldersTest : LanguageDependentTest
    {
        [Fact]
        public void ResultIsNoFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemId", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("someelement", "some value")))
                                    .Add("mappe", // Folder has no registration but has a subfolder
                                        new XmlElementHelper()
                                            .Add("mappe",
                                                new XmlElementHelper().Add("registrering",
                                                    new XmlElementHelper().Add("someelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ResultIsSomeFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemId", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("someelement", "some value")))
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("systemID", "mappeSysId1")
                                        .Add("mappeID", "mappeId1")
                                        .Add("tittel", "mappeTittel1")
                                        .Add("offentligTittel", "offentligMappeTittel1")))
                                .Add("mappe", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper() // Folder has neither registration or subfolder
                                        .Add("systemID", "mappeSysId2")
                                        .Add("mappeID", "mappeId2")
                                        .Add("tittel", "mappeTittel2")
                                        .Add("offentligTittel", "offentligMappeTittel2"))))))); 

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");

            testRun.TestResults.TestsResults[1].Message.Should()
                .Be("Mappe (systemID, mappeID, offentligTittel): mappeSysId1, mappeId1, offentligMappeTittel1");
            testRun.TestResults.TestsResults[2].Message.Should()
                .Be("Mappe (systemID, mappeID, offentligTittel): mappeSysId2, mappeId2, offentligMappeTittel2");

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ResultIsTwoFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemId", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper() // Folder has neither registration or subfolder
                                        .Add("someelement", "some value")
                                        .Add("systemID", "mappeSysId1")
                                        .Add("mappeID", "mappeId1")
                                        .Add("tittel", "mappeTittel1")
                                        .Add("offentligTittel", "offentligMappeTittel1"))
                                    .Add("mappe", new XmlElementHelper() // Folder has neither registration or subfolder
                                        .Add("someelement", "some value")
                                        .Add("systemID", "mappeSysId2")
                                        .Add("mappeID", "mappeId2")
                                        .Add("tittel", "mappeTittel2")
                                        .Add("offentligTittel", "offentligMappeTittel2"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 2");
            testRun.TestResults.TestsResults[1].Message.Should()
                .Be("Mappe (systemID, mappeID, offentligTittel): mappeSysId1, mappeId1, offentligMappeTittel1");
            testRun.TestResults.TestsResults[2].Message.Should()
                .Be("Mappe (systemID, mappeID, offentligTittel): mappeSysId2, mappeId2, offentligMappeTittel2");

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ResultIsTwoFoldersWithoutRegistrationsOrSubfoldersInOneOfTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", // Folder has neither registration or subfolder
                                    new XmlElementHelper()
                                        .Add("systemID", "mappeSysId1")
                                        .Add("mappeID", "mappeId1")
                                        .Add("tittel", "mappeTittel1")
                                        .Add("offentligTittel", "offentligMappeTittel1"))
                                .Add("mappe", // Folder has neither registration or subfolder
                                    new XmlElementHelper()
                                        .Add("systemID", "mappeSysId2")
                                        .Add("mappeID", "mappeId2")
                                        .Add("tittel", "mappeTittel2")
                                        .Add("offentligTittel", "offentligMappeTittel2")))))
                   .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()))
                                .Add("mappe", new XmlElementHelper() // Folder has no registration but has a subfolder
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper())))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.TestResults.TestResultSets[0].TestsResults.Should().Contain(r =>
                r.Message.Equals("Mappe (systemID, mappeID, offentligTittel): mappeSysId1, mappeId1, offentligMappeTittel1"));
            testRun.TestResults.TestResultSets[0].TestsResults.Should().Contain(r =>
                r.Message.Equals("Mappe (systemID, mappeID, offentligTittel): mappeSysId2, mappeId2, offentligMappeTittel2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldNotReportFoldersWithStatusUtgaar()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemId", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe", // Folder has neither registration or subfolder
                                        new XmlElementHelper().Add("someElement", "some value")
                                            .Add("systemId", "someMappeSystemId_1")
                                            .Add("mappeId", "someMappeId_1")
                                            .Add("tittel", "someMappeTitle_1")
                                            .Add("offentligTittel", "somePublicMappeTitle_1")
                                            .Add("saksstatus", "Utgår"))
                                    .Add("mappe", // Folder has neither registration or subfolder
                                        new XmlElementHelper().Add("someElement", "some value")
                                            .Add("systemId", "someMappeSystemId_2")
                                            .Add("mappeId", "someMappeId_2")
                                            .Add("tittel", "someMappeTitle_2")
                                            .Add("offentligTittel", "somePublicMappeTitle_2")
                                            .Add("saksstatus", "Avsluttet"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            List<TestResult> testResults = testRun.TestResults.TestsResults;

            testRun.TestResults.GetNumberOfResults().Should().Be(2);

            testResults.Count.Should().Be(2);

            testResults[0].Message.Should().Be("Totalt: 1");

            testResults[1].Message.Should().Be("Mappe (systemID, mappeID, offentligTittel): someMappeSystemId_2, someMappeId_2, somePublicMappeTitle_2");
        }
    }
}
