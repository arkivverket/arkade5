using System;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_44_NumberOfDisposalResolutionsTest : LanguageDependentTest
    {
        [Fact]
        public void HasSeverealDisposalResolutionsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("kassasjon", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))))));

            // Creating a test archive stating that it should contain disposal resolutions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_44_NumberOfDisposalResolutions(testArchive));


            testRun.Results.First().Message.Should().Be("Totalt: 7");
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Kassasjonsvedtak i arkivdel - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Kassasjonsvedtak i klasse - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Kassasjonsvedtak i mappe - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Kassasjonsvedtak i registrering - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"
            ));
            testRun.Results.Count.Should().Be(6);
        }

        [Fact]
        public void HasSeverealDisposalResolutionsOnSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someArchivePartTitle_1")
                                .Add("kassasjon", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty)))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someArchivePartTitle_2")
                                .Add("kassasjon", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))))));

            // Creating a test archive stating that it should contain disposal resolutions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_44_NumberOfDisposalResolutions(testArchive));

            testRun.Results.First().Message.Should().Be("Totalt: 14");
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Kassasjonsvedtak i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Kassasjonsvedtak i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Kassasjonsvedtak i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Kassasjonsvedtak i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Kassasjonsvedtak i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Kassasjonsvedtak i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Kassasjonsvedtak i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Kassasjonsvedtak i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Count.Should().Be(11);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedUpcomingDisposalsFalseAndActualUpcomingDisposalsTrue()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon", string.Empty))))))));


            // Creating a test archive stating that it should not contain any disposal resolutions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansFalse").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_44_NumberOfDisposalResolutions(testArchive));

            testRun.Results.First().Message.Should().Be("Totalt: 1");
            testRun.Results.Should()
                .Contain(r => r.Message.Equals("Kassasjonsvedtak i dokumentbeskrivelse - Antall: 1"));

            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Det er angitt at uttrekket ikke skal inneholde kassasjonsvedtak, men kassasjonsvedtak ble funnet"
                ) && r.Location.ToString().Equals("arkivuttrekk.xml"));

            testRun.Results.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedUpcomingDisposalsTrueAndActualUpcomingDisposalsFalse()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse", string.Empty)))))));


            // Creating a test archive stating that it should contain disposal resolutions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_44_NumberOfDisposalResolutions(testArchive));

            testRun.Results.First().Message.Should().Be("Totalt: 0");
            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Det er angitt at uttrekket skal inneholde kassasjonsvedtak, men ingen kassasjonsvedtak ble funnet"
                ) && r.Location.ToString().Equals("arkivuttrekk.xml"));

            testRun.Results.Count.Should().Be(2);
        }
    }
}
