using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_34_NumberOfMultiReferencedDocumentFilesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindMultiReferencedDocumentFilesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename2.docx")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_34_NumberOfMultiReferencedDocumentFiles());

            testRun.Results.First().Message.Should().Be("Totalt: 1");
            testRun.Results.Should()
                .Contain(r =>
                            r.Message.Equals("Referert dokumentfil: filename1.pdf - Antall referanser: 2")
                );
        }

        [Fact]
        public void ShouldFindMultiReferencedDocumentFilesInSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_1")
                            .Add("tittel", "someArchivePartTitle_1")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename1.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename2.docx"))))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename3.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename3.pdf")))))
                                        .Add("mappe",
                                            new XmlElementHelper()
                                                .Add("registrering", new[] { "xsi:type", "journalpost" },
                                                    new XmlElementHelper()
                                                        .Add("dokumentbeskrivelse",
                                                            new XmlElementHelper()
                                                                .Add("dokumentobjekt",
                                                                    new XmlElementHelper()
                                                                        .Add("referanseDokumentfil", "filename4.docx"))))))))
            );


            TestRun testRun = helper.RunEventsOnTest(new N5_34_NumberOfMultiReferencedDocumentFiles());


            testRun.Results.First().Message.Should().Be("Totalt: 2");
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals(
                            "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Referert dokumentfil: filename1.pdf - Antall referanser: 2")
                );
            testRun.Results.Should()
                .Contain(r =>
                        r.Message.Equals(
                            "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Referert dokumentfil: filename3.pdf - Antall referanser: 2")
                );
            testRun.Results.Count.Should().Be(3);
        }
    }
}
