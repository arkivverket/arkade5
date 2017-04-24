using Arkivverket.Arkade.Core;
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
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper()
                                        .Add("registrering", new[] {"xsi:type", "journalpost"},
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add("referanseDokumentfil",
                                                        "dokumenter/5000000.pdf"))))
                                        .Add("registrering", new[] {"xsi:type", "journalpost"},
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add("referanseDokumentfil",
                                                        // Backslashed file reference supported:
                                                        "dokumenter\\5000001.pdf"))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add("referanseDokumentfil",
                                                        // Subdirectory file reference:
                                                        "dokumenter/underkatalog/5000002.pdf")))))))));


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
                                        .Add("registrering", new[] {"xsi:type", "journalpost"},
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add("referanseDokumentfil",
                                                        "dokumenter/5000000.pdf"))))
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper().Add("dokumentbeskrivelse",
                                                new XmlElementHelper().Add("dokumentobjekt",
                                                    new XmlElementHelper().Add("referanseDokumentfil",
                                                        "dokumenter/underkatalog/5000002.pdf")))))))));


            TestRun testRun = CreateTestRun(xmlElementHelper);

            testRun.Results.Should().Contain(r => r.Message.Equals(
                    "Ikke-referert fil funnet: dokumenter/5000001.pdf")
            );

            testRun.Results.Count.Should().Be(1);
        }

        private static TestRun CreateTestRun(XmlElementHelper xmlElementHelper)
        {
            const string testdataDirectory = "TestData\\Noark5\\DocumentfilesControl\\FilesToBeReferenced";

            Archive testArchive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(testdataDirectory).Build();

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new DocumentfilesReferenceControl(testArchive));
            return testRun;
        }
    }
}
