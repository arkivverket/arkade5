using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_32_ControlDocumentFilesExistsTest : LanguageDependentTest
    {
        [Fact]
        public void ReferencedFilesExists()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf"))))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void ReferencedFilesAreMissing()
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
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000002.pdf"))))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("dokumentobjekt", new XmlElementHelper()
                                                .Add("referanseDokumentfil", "dokumenter/5000002.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            testRun.Results.Count.Should().Be(2);

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, someTitle_1 - Filen dokumenter/5000002.pdf ble ikke funnet"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, someTitle_2 - Filen dokumenter/5000002.pdf ble ikke funnet"));
        }

        [Fact]
        public void SlashOrBackSlashDirectorSeparatorIsIndifferent()
        {
            XmlElementHelper helper = new XmlElementHelper()
               .Add("arkiv", new XmlElementHelper()
                   .Add("arkivdel", new XmlElementHelper()
                       .Add("systemID", "someSystemId_1")
                       .Add("klassifikasjonssystem", new XmlElementHelper()
                           .Add("klasse", new XmlElementHelper()
                               .Add("mappe", new XmlElementHelper()
                                   .Add("registrering", new XmlElementHelper()
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter/5000000.pdf")))
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter\\5000001.pdf"))))))))
                   .Add("arkivdel", new XmlElementHelper()
                       .Add("systemID", "someSystemId_2")
                       .Add("klassifikasjonssystem", new XmlElementHelper()
                           .Add("klasse", new XmlElementHelper()
                               .Add("mappe", new XmlElementHelper()
                                   .Add("registrering", new XmlElementHelper()
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter\\5000000.pdf")))
                                       .Add("dokumentbeskrivelse", new XmlElementHelper()
                                           .Add("dokumentobjekt", new XmlElementHelper()
                                               .Add("referanseDokumentfil", "dokumenter/5000001.pdf")))))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "DocumentfilesControl", "ArchiveReferencedFiles")
            );

            TestRun testRun = helper.RunEventsOnTest(new N5_32_ControlDocumentFilesExists(testArchive));

            testRun.Results.Count.Should().Be(0);
        }
    }
}
