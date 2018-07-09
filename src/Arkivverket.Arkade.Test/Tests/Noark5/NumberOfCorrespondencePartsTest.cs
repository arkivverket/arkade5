using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfCorrespondencePartsTest
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
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("1");
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
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCorrespondenceParts());

            testRun.Results[0].Message.Should().Be("0");
        }

        [Fact]
        public void NumberOfCorrespondencePartsIsTwoOneInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()
                                            .Add("korrespondansepart", new XmlElementHelper())))))
                        .Add("arkivdel", new XmlElementHelper()
                            .Add("systemID", "someSystemId_2")
                            .Add("klassifikasjonssystem", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new[] {"xsi:type", "journalpost"},
                                            new XmlElementHelper()
                                                .Add("korrespondansepart", new XmlElementHelper()))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCorrespondenceParts());

            testRun.Results.Should().Contain(r => r.Message.Equals("2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_2: 1"));
        }
    }
}
