using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest
    {
        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem", string.Empty)
                    )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfClassificationSystems());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem", string.Empty))
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem", string.Empty))
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfClassificationSystems());

            testRun.Results.First().Message.Should().Contain("2");
        }
    }
}