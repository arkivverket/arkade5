using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_16_NumberOfRegistrationsTest : LanguageDependentTest
    {
        [Fact]
        public void HasSeveralRegistrationsOnSingleArchivePart()
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
                                                            new[] {"xsi:type", "journalpost"},
                                                            ""
                                                        )
                                                        .Add("registrering",
                                                            new[] {"xsi:type", "moete"},
                                                            ""
                                                        )
                                                        .Add("registrering",
                                                            // No spesific type
                                                            ""
                                                        )
                                                        .Add("registrering",
                                                            new[] {"xsi:type", "journalpost"},
                                                            ""
                                                        ))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new[] {"xsi:type", "journalpost"},
                                                            ""
                                                        ))))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "RegistrationControl", "FiveRegistrations")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_16_NumberOfRegistrations(testArchive));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 5"));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Registreringstype: journalpost - Antall: 3"));
            testResults.Should().Contain(r => r.Message.Equals("Registreringstype: moete - Antall: 1"));
            testResults.Should().Contain(r => r.Message.Equals("Registreringstype: registrering - Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(4);
        }

        [Fact]
        public void HasSeveralRegistrationsOnTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
               .Add("arkiv",
                   new XmlElementHelper()
                       .Add("arkivdel",
                           new XmlElementHelper()
                               .Add("systemID", "someArchivePartSystemId_1")
                               .Add("tittel", "someArchivePartTittel_1")
                               .Add("klassifikasjonssystem",
                                   new XmlElementHelper()
                                       .Add("klasse",
                                           new XmlElementHelper()
                                               .Add("mappe",
                                                   new XmlElementHelper()
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       )
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "moete" },
                                                           ""
                                                       )
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       ))
                                               .Add("mappe",
                                                   new XmlElementHelper()
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       )))))
                       .Add("arkivdel",
                           new XmlElementHelper()
                               .Add("systemID", "someArchivePartSystemId_2")
                               .Add("tittel", "someArchivePartTittel_2")
                               .Add("klassifikasjonssystem",
                                   new XmlElementHelper()
                                       .Add("klasse",
                                           new XmlElementHelper()
                                               .Add("mappe",
                                                   new XmlElementHelper()
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       )
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "moete" },
                                                           ""
                                                       )
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       )
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "moete" },
                                                           ""
                                                       ))
                                               .Add("mappe",
                                                   new XmlElementHelper()
                                                       .Add("registrering",
                                                           new[] { "xsi:type", "journalpost" },
                                                           ""
                                                       ))))));
            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "RegistrationControl", "NineRegistrations")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_16_NumberOfRegistrations(testArchive));

            testRun.TestResults.TestsResults.Should().Contain(r => r.Message.Equals("Totalt: 9"));

            List<TestResult> arkivdel1Results = testRun.TestResults.TestResultSets[0].TestsResults;
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Antall: 4"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Registreringstype: journalpost - Antall: 3"));
            arkivdel1Results.Should().Contain(r => r.Message.Equals("Registreringstype: moete - Antall: 1"));

            List<TestResult> arkivdel2Results = testRun.TestResults.TestResultSets[1].TestsResults;
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Antall: 5"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Registreringstype: journalpost - Antall: 3"));
            arkivdel2Results.Should().Contain(r => r.Message.Equals("Registreringstype: moete - Antall: 2"));

            testRun.TestResults.GetNumberOfResults().Should().Be(7);
        }

        [Fact]
        public void DocumentedAndFoundNumberOfRegistrationsMismatchShouldTriggerWarning()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("mappe",
                                new XmlElementHelper()
                                    .Add("registrering",
                                        new [] {"xsi:type", "journalpost"},
                                        ""
                                        )))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "RegistrationControl", "FiveRegistrations")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_16_NumberOfRegistrations(testArchive));

            List<TestResult> testResults = testRun.TestResults.TestsResults;
            testResults.Should().Contain(r => r.Message.Equals("Totalt: 1"));
            testResults.Should().Contain(r => r.Message.Equals(
                "Det er angitt at arkivstrukturen skal innholde 5 registreringer, men 1 ble funnet"));
            testResults.Should().Contain(r => r.Message.Equals("Registreringstype: journalpost - Antall: 1"));

            testRun.TestResults.GetNumberOfResults().Should().Be(3);
        }

        [Fact]
        public void HasNoRegistrationsInTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
               .Add("arkiv",
                   new XmlElementHelper()
                       .Add("arkivdel",
                           new XmlElementHelper()
                               .Add("systemID", "someArchivePartSystemId_1")
                               .Add("tittel", "someArchivePartTittel_1")
                               .Add("klassifikasjonssystem",
                                   new XmlElementHelper()
                                       .Add("klasse",
                                           new XmlElementHelper()
                                               .Add("mappe",
                                                   new XmlElementHelper())
                                               .Add("mappe",
                                                   new XmlElementHelper()))))
                       .Add("arkivdel",
                           new XmlElementHelper()
                               .Add("systemID", "someArchivePartSystemId_2")
                               .Add("tittel", "someArchivePartTittel_2")
                               .Add("klassifikasjonssystem",
                                   new XmlElementHelper()
                                       .Add("klasse",
                                           new XmlElementHelper()
                                               .Add("mappe",
                                                   new XmlElementHelper())
                                               .Add("mappe",
                                                   new XmlElementHelper())))));

            Archive testArchive = TestUtil.CreateArchiveExtraction(
                Path.Combine("TestData", "Noark5", "RegistrationControl", "ZeroRegistrations")
            );
            TestRun testRun = helper.RunEventsOnTest(new N5_16_NumberOfRegistrations(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be("Totalt: 0");

            testRun.TestResults.GetNumberOfResults().Should().Be(1);
        }
    }
}