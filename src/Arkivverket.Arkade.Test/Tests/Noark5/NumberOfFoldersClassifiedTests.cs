using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;


namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersClassifiedTests
    {
        [Fact]
        public void ShouldReturnOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                        .Add("arkivdel", new XmlElementHelper().Add("tittel", "test")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                        .Add("mappe", string.Empty)))
                        )
                );


            TestRun testRun = helper.RunEventsOnTest(new NumberOfFoldersClassified());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void ShouldReturnTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                        .Add("arkivdel", new XmlElementHelper().Add("tittel", "test")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                        .Add("klasse", new XmlElementHelper()
                        .Add("mappe", string.Empty)
                        .Add("mappe", string.Empty))))
                );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFoldersClassified());
            var x = testRun.Results.First().Message;
            testRun.Results.First().Message.Should().Contain("2");
        }
    }
}
