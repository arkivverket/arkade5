﻿using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class DocumentfilesReferenceControlTest
    {
        [Fact]
        public void AllFilesAreReferenced()
        {
            /*
            The documents directory does not use the default/fallback name (dokumenter).
            One of the references use backslashes in the path.
            2 of the referenced files are located in a documents directory subdirectory.
            The referenced files have all combinations of lowercase/uppercase in their filename + extension.
            */

            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/a.pdf"
                                                    ))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/B.PDF"
                                                    ))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/underkatalog/C.pdf"
                                                    ))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/underkatalog/d.PDF"
                                                    )))))))));

            TestRun testRun = CreateTestRun(xmlElementHelper);

            testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void SomeFilesAreNotReferenced()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/a.pdf"
                                                    ))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/B.PDF"
                                                    ))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add(
                                                        "referanseDokumentfil", "DOKUMENT/underkatalog/C.pdf"
                                                    )))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Ikke-referert fil funnet: DOKUMENT/underkatalog/d.PDF")
            );

            testRun.Results.Count.Should().Be(1);
        }

        private static TestRun CreateTestRun(XmlElementHelper xmlElementHelper)
        {
            const string testdataDirectory = "TestData/Noark5/DocumentfilesControl/FilesToBeReferenced";

            Archive testArchive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(testdataDirectory).Build();

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new DocumentfilesReferenceControl(testArchive));
            return testRun;
        }
    }
}
