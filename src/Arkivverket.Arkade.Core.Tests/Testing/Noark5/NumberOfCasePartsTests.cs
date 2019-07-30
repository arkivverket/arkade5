using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;


namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class NumberOfCasePartsTests
    {
        [Fact]
        public void NumberOfCasePartsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts());

            testRun.Results[0].Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void NumberOfCasePartsIsOnePerArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("systemID", "journpost57d6608569ed33.70652483"))))))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_3")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("sakspart", new XmlElementHelper()
                                            .Add("sakspartID", "Sakspart1")))))))
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_35_NumberOfCaseParts());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID) someSystemId_3: 1"));
        }
    }
}