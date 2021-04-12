using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_41_NumberOfDocumentFlowsTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfDocumentFlowsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("dokumentflyt", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_41_NumberOfDocumentFlows());

            testRun.Results[0].Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void NumberOfDocumentFlowsIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_41_NumberOfDocumentFlows());

            testRun.Results[0].Message.Should().Be("Totalt: 0");
        }

        [Fact]
        public void NumberOfDocumentFlowsIsOnePerArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"}, new XmlElementHelper()
                                        .Add("dokumentflyt", new XmlElementHelper()))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new[] {"xsi:type", "journalpost"},
                                        new XmlElementHelper()
                                            .Add("dokumentflyt", new XmlElementHelper())))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_41_NumberOfDocumentFlows());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_1 - someTitle_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_2 - someTitle_2: 1"));
        }
    }
}
