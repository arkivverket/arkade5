using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_42_NumberOfRestrictionsTest : LanguageDependentTest
    {
        [Fact]
        public void HasSeveralRestrictionsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))))));

            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_42_NumberOfRestrictions(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 7");
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i arkivdel - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i klasse - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i mappe - Antall: 2"));
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i registrering - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i dokumentbeskrivelse - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(6);
        }

        [Fact]
        public void HasSeveralRestrictionsOnSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("tittel", "someArchivePartTitle_1")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty)))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("tittel", "someArchivePartTitle_2")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))))));

            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_42_NumberOfRestrictions(testArchive));


            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 14");

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someArchivePartSystemId_1"))?.TestsResults;
            arkivdel1Results?.First().Message.Should().Be("Antall: 7");
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Skjerminger i arkivdel - Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Skjerminger i klasse - Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Skjerminger i mappe - Antall: 2"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Skjerminger i registrering - Antall: 1"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Skjerminger i dokumentbeskrivelse - Antall: 2"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets
                .Find(s => s.Name.Contains("someArchivePartSystemId_1"))?.TestsResults;
            arkivdel2Results?.First().Message.Should().Be("Antall: 7");
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Skjerminger i arkivdel - Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Skjerminger i klasse - Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Skjerminger i mappe - Antall: 2"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Skjerminger i registrering - Antall: 1"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Skjerminger i dokumentbeskrivelse - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(13);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedRestrictionsFalseAndActualRestrictionsTrue()
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
                                                                        .Add("skjerming", string.Empty))))))));


            // Creating a test archive stating that it should not contain any restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansFalse").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_42_NumberOfRestrictions(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.First().Message.Should().Be("Totalt: 1");
            testResults.Should().Contain(r => r.Message.Equals("Skjerminger i dokumentbeskrivelse - Antall: 1"));
            testResults.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Det er angitt at uttrekket ikke skal inneholde skjerminger, men skjerminger ble funnet") &&
                r.Location.ToString().Equals("arkivuttrekk.xml - linje(r): 0"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedRestrictionsTrueAndActualRestrictionsFalse()
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


            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_42_NumberOfRestrictions(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");
            testRun.TestResults.TestsResults.Should().Contain(r =>
                r.IsError() &&
                r.Message.Equals(
                    "Det er angitt at uttrekket skal inneholde skjerminger, men ingen skjerminger ble funnet") &&
                r.Location.ToString().Equals("arkivuttrekk.xml"));

            testRun.TestResults.TestsResults.Count.Should().Be(2);
        }

        [Fact]
        public void HasNoRestrictionsOnSingleArchivePart()
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
                                                                    new XmlElementHelper())))))));


            // Creating a test archive stating that it should not contain any restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData\\Noark5\\MetaDataTesting\\BooleansFalse").Build();

            TestRun testRun = helper.RunEventsOnTest(new N5_42_NumberOfRestrictions(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}
