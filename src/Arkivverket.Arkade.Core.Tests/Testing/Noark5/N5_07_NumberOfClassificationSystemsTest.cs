using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_07_NumberOfClassificationSystemsTest
    {
        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", string.Empty)
                    )
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_07_NumberOfClassificationSystems());

            testRun.Results.First().Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", string.Empty))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("klassifikasjonssystem", string.Empty))
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_07_NumberOfClassificationSystems());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall klassifikasjonssystemer i arkivdel (systemID) someSystemId_1: 1"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall klassifikasjonssystemer i arkivdel (systemID) someSystemId_2: 1"));
        }
    }
}
