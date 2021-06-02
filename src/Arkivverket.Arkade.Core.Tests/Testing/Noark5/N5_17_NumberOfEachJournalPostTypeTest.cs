using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_17_NumberOfEachJournalPostTypeTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindSeveralJournalpostTypesInSeveralArchiveParts()
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
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_1")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Inngående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_2")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_3")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_4")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Saksframlegg"))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_5")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype",
                                                        "Organinternt dokument for oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_6")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Organinternt dokument uten oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_7")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Saksframlegg")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_17_NumberOfEachJournalPostType());


            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalposttype: Inngående dokument - Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalposttype: Utgående dokument - Antall: 2"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Journalposttype: Saksframlegg - Antall: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r =>
                r.Message.Equals("Journalposttype: Organinternt dokument for oppfølging - Antall: 1"));
            arkivdel2Results.Should().Contain(r =>
                r.Message.Equals("Journalposttype: Organinternt dokument uten oppfølging - Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Journalposttype: Saksframlegg - Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void ShouldFindSeveralJournalpostTypesInSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_3")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_8")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Inngående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_9")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_10")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_11")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Saksframlegg")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_12")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Organinternt dokument for oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_13")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Organinternt dokument uten oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_14")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Saksframlegg")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_17_NumberOfEachJournalPostType());


            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Inngående dokument - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Utgående dokument - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals(
                "Journalposttype: Organinternt dokument for oppfølging - Antall: 1"
            ));
            testResults.Should().Contain(r => r.Message.Equals(
                "Journalposttype: Organinternt dokument uten oppfølging - Antall: 1"
            ));
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Saksframlegg - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldFindSeveralJournalpostTypesInSingleArchivePartSomeWithoutMainDocument()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_4")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_15")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Inngående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_16")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_17")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Utgående dokument")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_18")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", "Saksframlegg")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_19")
                                                    .Add("journalposttype", "Organinternt dokument for oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_20")
                                                    .Add("journalposttype", "Organinternt dokument uten oppfølging")))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_21")
                                                    .Add("journalposttype", "Saksframlegg")))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_17_NumberOfEachJournalPostType());


            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Inngående dokument - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Utgående dokument - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals(
                "Journalposttype: Organinternt dokument for oppfølging - Antall: 1"
            ));
            testResults.Should().Contain(r => r.Message.Equals(
                "Journalposttype: Organinternt dokument uten oppfølging - Antall: 1"
            ));
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype: Saksframlegg - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(5);
        }

        [Fact]
        public void ShouldHandleEmptyOrAbsentJournalPostTypes()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_3")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_8")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))
                                                    .Add("journalposttype", string.Empty))) // Type empty
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering",
                                                new[] {"xsi:type", "journalpost"},
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_9")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper().Add("tilknyttetRegistreringSom",
                                                            "Hoveddokument")))))))); // No type element


            TestRun testRun = helper.RunEventsOnTest(new N5_17_NumberOfEachJournalPostType());


            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Journalposttype:  - Antall: 1") && r.IsError());

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }

        [Fact]
        public void ShouldFindNoJournalPostTypesInAnyArchiveParts()
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
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_1")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_2")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_3")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_4")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument")))))))
                    .Add("arkivdel",
                        new XmlElementHelper()
                            .Add("systemID", "someArchivePartSystemId_2")
                            .Add("tittel", "someArchivePartTitle_2")
                            .Add("klassifikasjonssystem",
                                new XmlElementHelper().Add("klasse",
                                    new XmlElementHelper()
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_5")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_6")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))
                                        .Add("mappe",
                                            new XmlElementHelper().Add("registrering", new[] { "xsi:type", "journalpost" },
                                                new XmlElementHelper()
                                                    .Add("systemID", "someJournalPostSystemId_7")
                                                    .Add("dokumentbeskrivelse",
                                                        new XmlElementHelper()
                                                            .Add("tilknyttetRegistreringSom", "Hoveddokument"))))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_17_NumberOfEachJournalPostType());

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
