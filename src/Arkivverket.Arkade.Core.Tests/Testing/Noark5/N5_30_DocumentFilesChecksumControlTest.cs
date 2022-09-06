﻿using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_30_DocumentFilesChecksumControlTest : LanguageDependentTest
    {
        [Fact]
        public void ActualAndDocumentedChecksumsDoMatch()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "archivePartSystemId_1")
                        .Add("tittel", "archivePartTitle_1")
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper() // "/" is supported:
                                                        .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                        .Add("sjekksum", // Actual checksum of testdata file:
                                                            "3B29DFCC4286E50B180AF8F21904C86F8AA42A23C4055C3A71D0512F9AE3886F")
                                                        .Add("sjekksumAlgoritme", "SHA-256")
                                                )))
                                        .Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper() // "\" is supported:
                                                        .Add("referanseDokumentfil", "dokumenter\\5000001.pdf")
                                                        .Add("sjekksum", // Actual checksum of testdata file:
                                                            "2EA3A86DE226D791A07ABB5279B0E2813B037730730BA630DE4F14DEA7C32208")
                                                        .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void ActualAndDocumentedChecksumsDoNotMatch()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel", new XmlElementHelper()
                            .Add("systemID", "archivePartSystemId_1")
                            .Add("tittel", "archivePartTitle_1")
                            .Add("klassifikasjonssystem", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("systemID", "someSystemId_1")
                                                .Add("dokumentobjekt", new XmlElementHelper()
                                                    .Add("referanseDokumentfil", "dokumenter\\5000000.pdf")
                                                    .Add("sjekksum", "someNotMatchingCheckSum")
                                                    .Add("sjekksumAlgoritme", "SHA-256"))))
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("systemID", "someSystemId_2")
                                                .Add("dokumentobjekt", new XmlElementHelper()
                                                    .Add("referanseDokumentfil", "dokumenter/5000001.pdf")
                                                    .Add("sjekksum", "someNotMatchingCheckSum")
                                                    .Add("sjekksumAlgoritme", "SHA-256"))))))))
                        .Add("arkivdel", new XmlElementHelper().Add("systemID", "archivePartSystemId_2")
                            .Add("tittel", "archivePartTitle_2")
                            .Add("klassifikasjonssystem", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper()
                                            .Add("dokumentbeskrivelse", new XmlElementHelper()
                                                .Add("systemID", "someSystemId_3")
                                                .Add("dokumentobjekt", new XmlElementHelper()
                                                    .Add("referanseDokumentfil", "dokumenter\\5000000.pdf")
                                                    .Add("sjekksum", "someNotMatchingCheckSum")
                                                    .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter\\5000000.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_1"
                ));
            arkivdel1Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter/5000001.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_2"
                ));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter\\5000000.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_3"
                ));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ActualAndDocumentedChecksumsDoNotMatchInArchiveWithOneArchivePart()
        {
            XmlElementHelper xmlElementHelper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "archivePartSystemId_1")
                        .Add("tittel", "archivePartTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "someSystemId_1")
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                // "/" is supported:
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                .Add("sjekksum", "someNotMatchingCheckSum")
                                                .Add("sjekksumAlgoritme", "SHA-256"))))
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "someSystemId_2")
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                // "\" is supported:
                                                .Add("referanseDokumentfil", "dokumenter\\5000001.pdf")
                                                .Add("sjekksum", "someNotMatchingCheckSum")
                                                .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter/5000000.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_1"
                ));
            testResults.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter\\5000001.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_2"
                ));

            testRun.TestResults.GetNumberOfResults().Should().Be(2);
        }

        [Fact]
        public void ShouldShowEmptyDocumentfileReferenceMessageWhenEmpty()
        {
            XmlElementHelper xmlElementHelper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "archivePartSystemId_1")
                        .Add("tittel", "archivePartTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "someSystemId_1")
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                // "/" is supported:
                                                .Add("referanseDokumentfil", "")
                                                .Add("sjekksum", // Actual checksum of testdata file:
                                                    "3B29DFCC4286E50B180AF8F21904C86F8AA42A23C4055C3A71D0512F9AE3886F")
                                                .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            List<TestResult> testResults = testRun.TestResults.TestsResults;

            testResults.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Verdien til \"referanseDokumentfil\" er ikke gyldig"
                ));

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        private static TestRun CreateTestRun(XmlElementHelper xmlElementHelper)
        {
            const string testdataDirectory = "TestData\\Noark5\\DocumentfilesControl\\FilesWithDocumentedChecksums";

            Archive testArchive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(testdataDirectory)
                .Build();

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_30_DocumentFilesChecksumControl(testArchive));
            return testRun;
        }
    }
}
