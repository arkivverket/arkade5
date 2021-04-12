using System.IO;
using Arkivverket.Arkade.Core.Base;
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Totalt: 5"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Registreringstype: journalpost - Antall: 3"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Registreringstype: moete - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Registreringstype: registrering - Antall: 1"
            ));
            testRun.Results.Count.Should().Be(4);
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Totalt: 9"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_1, someArchivePartTittel_1 - Totalt: 4"));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID, tittel): someArchivePartSystemId_2, someArchivePartTittel_2 - Totalt: 5"));
            testRun.Results.Count.Should().Be(7);
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

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Det er angitt at arkivstrukturen skal innholde 5 registreringer, men 1 ble funnet"
            ));
        }
    }
}