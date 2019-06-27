using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfRegistrationsTest
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
                                                            new[] {"xsi:type", "journalpost"},
                                                            ""
                                                        ))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new[] {"xsi:type", "journalpost"},
                                                            ""
                                                        ))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfRegistrations());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Totalt: 4"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Registertype: journalpost - Antall: 3"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Registertype: moete - Antall: 1"
            ));
            testRun.Results.Count.Should().Be(3);
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
            TestRun testRun = helper.RunEventsOnTest(new NumberOfRegistrations());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Totalt: 9"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Totalt: 4"));
            testRun.Results.Count.Should().Be(7);
        }
    }
}