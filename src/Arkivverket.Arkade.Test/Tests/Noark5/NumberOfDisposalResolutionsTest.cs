using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfDisposalResolutionsTest
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
                                .Add("kassasjon",
                                    new XmlElementHelper()
                                        .Add("kassasjonsvedtak", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon",
                                                    new XmlElementHelper()
                                                        .Add("kassasjonsvedtak", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjonsvedtak", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Strengt hemmelig")))))))));


            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalResolutions());

            testRun.Results.Should().Contain(r => r.Message.Equals("Kassasjonsvedtak i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Kassasjonsvedtak i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Kassasjonsvedtak i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Kassasjonsvedtak i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Count.Should().Be(5);
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
                                .Add("kassasjon",
                                    new XmlElementHelper()
                                        .Add("kassasjonsvedtak", "Kasseres"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon",
                                                    new XmlElementHelper()
                                                        .Add("kassasjonsvedtak", "Kasseres"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Kasseres"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjonsvedtak", "Kasseres"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Kasseres")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Kasseres"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Kasseres"))))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("kassasjon",
                                    new XmlElementHelper()
                                        .Add("kassasjonsvedtak", "Kasseres"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("kassasjon",
                                                    new XmlElementHelper()
                                                        .Add("kassasjonsvedtak", "Kasseres"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Kasseres"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("kassasjon",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjonsvedtak", "Kasseres"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Kasseres")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("kassasjon",
                                                            new XmlElementHelper()
                                                                .Add("kassasjonsvedtak", "Kasseres"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("kassasjon",
                                                                            new XmlElementHelper()
                                                                                .Add("kassasjonsvedtak", "Kasseres")))))))));


            TestRun testRun = helper.RunEventsOnTest(new NumberOfDisposalResolutions());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Kassasjonsvedtak i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Kassasjonsvedtak i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Kassasjonsvedtak i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Kassasjonsvedtak i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Kassasjonsvedtak i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Kassasjonsvedtak i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Kassasjonsvedtak i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Kassasjonsvedtak i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Kassasjonsvedtak i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Count.Should().Be(10);
        }
    }
}
