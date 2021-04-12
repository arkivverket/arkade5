using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_04_NumberOfArchivesTest : LanguageDependentTest
    {
        [Fact]
        public void TwoArchivesAreFound()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", string.Empty)
                .Add("arkiv", string.Empty);

            TestRun testRun = helper.RunEventsOnTest(new N5_04_NumberOfArchives());

            testRun.Results.First().Message.Should().Be("Totalt: 2");

            testRun.Results.Count.Should().Be(1);
        }

        [Fact]
        public void FiveArchivesOnThreeLevelsAreFound()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", string.Empty)
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkiv", new XmlElementHelper()
                        .Add("arkiv", string.Empty))
                    .Add("arkiv", string.Empty));

            TestRun testRun = helper.RunEventsOnTest(new N5_04_NumberOfArchives());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 5"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall arkiv på nivå 1: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall arkiv på nivå 2: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall arkiv på nivå 3: 1"));

            testRun.Results.Count.Should().Be(4);
        }
    }
}
