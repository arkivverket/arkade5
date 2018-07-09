using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.Noark5;
using Arkivverket.Arkade.Test.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDisposalsExecutedTest
    {
        [Fact]
        public void HasSeverealDisposalsExecutedWithinSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("utfoertKassasjon", string.Empty)
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
                                                                        .Add("utfoertKassasjon", string.Empty))))))));


            // Creating a test archive stating that it should contain executed disposals
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalsExecuted(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall utførte kassasjoner: 2"
            ));
            testRun.Results.Count.Should().Be(1);
        }

        [Fact]
        public void HasSeverealDisposalsExecutedWithinSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("utfoertKassasjon", string.Empty)
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
                                                                        .Add("utfoertKassasjon", string.Empty)))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
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
                                                                        .Add("utfoertKassasjon", string.Empty))))))));

            // Creating a test archive stating that it should contain executed disposals
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalsExecuted(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Antall utførte kassasjoner: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Antall utførte kassasjoner: 1"
            ));
            testRun.Results.Count.Should().Be(2);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedExecutedDisposalsFalseAndActualExecutedDisposalsTrue()
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
                                                                        .Add("utfoertKassasjon", string.Empty))))))));


            // Creating a test archive stating that it should not contain any executed disposals
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansFalse").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalsExecuted(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Antall utførte kassasjoner: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er dokumentert at uttrekket ikke skal omfatte utførte kassasjoner, men utførte kassasjoner ble funnet"
            ));
            testRun.Results.Count.Should().Be(2);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedExecutedDisposalsTrueAndActualExecutedDisposalsFalse()
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


            // Creating a test archive stating that it should contain executed disposals
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalsExecuted(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er dokumentert at uttrekket skal omfatte utførte kassasjoner, men ingen utførte kassasjoner ble funnet"
            ));
            testRun.Results.Count.Should().Be(1);
        }
    }
}
