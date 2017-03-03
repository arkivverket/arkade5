using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class SystemIdUniqueControlTest
    {
        [Fact]
        public void EverySystemIdIsUnique()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someSystemId_2")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("systemID", "someSystemId_3")
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("systemID", "someSystemId_4")
                                                .Add("klasse",
                                                    new XmlElementHelper()
                                                        .Add("systemID", "someSystemId_5")
                                                        .Add("mappe",
                                                            new XmlElementHelper()
                                                                .Add("systemID", "someSystemId_6")
                                                                .Add("registrering",
                                                                    new XmlElementHelper()
                                                                        .Add("systemID", "someSystemId_7")
                                                                        .Add("dokumentbeskrivelse",
                                                                            new XmlElementHelper()
                                                                                .Add("systemID", "someSystemId_8")))))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new SystemIdUniqueControl());

            testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void NotEverySystemIdIsUnique()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someSystemId_2")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("systemID", "someSystemId_3")
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("systemID", "someSystemId_4")
                                                .Add("klasse",
                                                    new XmlElementHelper()
                                                        .Add("systemID", "someSystemId_4")
                                                        .Add("mappe",
                                                            new XmlElementHelper()
                                                                .Add("systemID", "someSystemId_5")
                                                                .Add("registrering",
                                                                    new XmlElementHelper()
                                                                        .Add("systemID", "someSystemId_5")
                                                                        .Add("dokumentbeskrivelse",
                                                                            new XmlElementHelper()
                                                                                .Add("systemID", "someSystemId_5")))))))));

            TestRun testRun = xmlElementHelper.RunEventsOnTest(new SystemIdUniqueControl());

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Ikke-unik ID: Systemidentifikasjonen (systemID) someSystemId_4 forekommer 2 ganger"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Ikke-unik ID: Systemidentifikasjonen (systemID) someSystemId_5 forekommer 3 ganger"
            ));
            testRun.Results.Count.Should().Be(2);
        }
    }
}
