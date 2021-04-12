using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_05_NumberOfArchivePartsTests : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindTwoArchiveparts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("systemID", "someArchiveSystemId_1")
                    .Add("arkivdel", string.Empty)
                    .Add("arkivdel", string.Empty)
                );

            TestRun testRun = helper.RunEventsOnTest(new N5_05_NumberOfArchiveParts());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
        }

        [Fact]
        public void ShouldFindThreeArchivepartsInTwoArchives()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("systemID", "someArchiveSystemId_1")
                    .Add("arkivdel", string.Empty)
                    .Add("arkivdel", string.Empty))
                .Add("arkiv", new XmlElementHelper()
                    .Add("systemID", "someArchiveSystemId_2")
                    .Add("arkivdel", string.Empty));

            TestRun testRun = helper.RunEventsOnTest(new N5_05_NumberOfArchiveParts());

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Totalt: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall arkivdeler i arkiv (systemID) someArchiveSystemId_1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Antall arkivdeler i arkiv (systemID) someArchiveSystemId_2: 1"));
        }
    }
}
