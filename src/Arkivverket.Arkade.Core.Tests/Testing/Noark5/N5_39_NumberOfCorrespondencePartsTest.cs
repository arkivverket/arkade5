using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_39_NumberOfCorrespondencePartsTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfCorrespondencePartsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_39_NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void NumberOfCorrespondencePartsIsTwoInSameJournalPost()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())
                                            .Add("korrespondansepart", new XmlElementHelper()
                                            )))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_39_NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("Totalt: 2");
        }

        [Fact]
        public void NumberOfCorrespondencePartsIsTwoInTwoDifferentRegistrationTypes()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())
                                    ).Add("registrering", // No specific type
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())
                                    ))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_39_NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("Totalt: 2");
        }

        [Fact]
        public void NumberOfCorrespondencePartsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_39_NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("Totalt: 0");
        }

        [Fact]
        public void NumberOfCorrespondencePartsIsTwoOneInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] { "xsi:type", "journalpost" },
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper()))))))
                        .Add("arkivdel", new XmlElementHelper()
                            .Add("systemID", "someSystemId_2")
                            .Add("tittel", "someTitle_2")
                            .Add("klassifikasjonssystem", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new[] { "xsi:type", "journalpost" },
                                            new XmlElementHelper()
                                                .Add("korrespondansepart", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_39_NumberOfCorrespondenceParts());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_1 - someTitle_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_2 - someTitle_2: 1"));
        }
    }
}
