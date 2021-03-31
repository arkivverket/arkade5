using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_43_NumberOfClassificationsTest : LanguageDependentTest
    {
        [Fact]
        public void HasSeverealClassificationsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_43_NumberOfClassifications());

            testRun.Results.First().Message.Should().Be("Totalt: 7");
            testRun.Results.Should().Contain(r => r.Message.Equals("Graderinger i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Graderinger i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Graderinger i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Graderinger i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Graderinger i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Count.Should().Be(6);
        }

        [Fact]
        public void HasSeverealClassificationsOnSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someArchivePartTitle_1")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig"))))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someArchivePartTitle_2")
                                .Add("gradering",
                                    new XmlElementHelper()
                                        .Add("gradering", "Strengt hemmelig"))
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("gradering",
                                                    new XmlElementHelper()
                                                        .Add("gradering", "Strengt hemmelig"))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("gradering",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering", "Strengt hemmelig"))
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("gradering",
                                                            new XmlElementHelper()
                                                                .Add("gradering", "Strengt hemmelig"))
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("gradering",
                                                                            new XmlElementHelper()
                                                                                .Add("gradering", "Strengt hemmelig")))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_43_NumberOfClassifications());

            testRun.Results.First().Message.Should().Be("Totalt: 14");
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Graderinger i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Graderinger i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Graderinger i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Graderinger i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTitle_1 - Graderinger i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Graderinger i arkivdel - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Graderinger i klasse - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Graderinger i mappe - Antall: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Graderinger i registrering - Antall: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTitle_2 - Graderinger i dokumentbeskrivelse - Antall: 2"));
            testRun.Results.Count.Should().Be(11);
        }
    }
}
