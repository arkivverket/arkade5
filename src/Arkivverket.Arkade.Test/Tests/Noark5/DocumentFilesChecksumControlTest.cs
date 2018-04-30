﻿using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class DocumentFilesChecksumControlTest
    {
        [Fact]
        public void ActualAndDocumentedChecksumsDoMatch()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
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
                                                        .Add("referanseDokumentfil", "dokumenter/5000001.pdf")
                                                        .Add("sjekksum", // Actual checksum of testdata file:
                                                            "2EA3A86DE226D791A07ABB5279B0E2813B037730730BA630DE4F14DEA7C32208")
                                                        .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            // testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void ActualAndDocumentedChecksumsDoNotMatch()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper()
                                                    .Add("systemID", "someSystemId_1")
                                                    .Add("dokumentobjekt",
                                                        new XmlElementHelper()
                                                            .Add("referanseDokumentfil", "dokumenter/5000000.pdf")
                                                            .Add("sjekksum", "someNotMatchingCheckSum")
                                                            .Add("sjekksumAlgoritme", "SHA-256"))))
                                        .Add("registrering",
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper()
                                                    .Add("systemID", "someSystemId_2")
                                                    .Add("dokumentobjekt",
                                                        new XmlElementHelper()
                                                            .Add("referanseDokumentfil", "dokumenter/5000001.pdf")
                                                            .Add("sjekksum", "someNotMatchingCheckSum")
                                                            .Add("sjekksumAlgoritme", "SHA-256")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter/5000000.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_1"
                ));

            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Filen dokumenter/5000001.pdf har ikke samme sjekksum som oppgitt i dokumentbeskrivelse (systemID) someSystemId_2"
                ));

            testRun.Results.Count.Should().Be(2);
        }

        private static TestRun CreateTestRun(XmlElementHelper xmlElementHelper)
        {
            const string testdataDirectory = "TestData/Noark5/DocumentfilesControl/FilesWithDocumentedChecksums";

            Archive testArchive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(testdataDirectory)
                .Build();

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new DocumentFilesChecksumControl(testArchive));
            return testRun;
        }
    }
}
