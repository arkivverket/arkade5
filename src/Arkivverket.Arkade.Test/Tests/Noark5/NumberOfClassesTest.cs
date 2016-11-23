using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassesTest
    {
        [Fact]
        public void NumberOfClassesIsFour()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", string.Empty)
                                .Add("klasse", string.Empty)
                        )
                    )
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse", string.Empty)
                        )
                    )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfClasses());

            testRun.Results.First().Message.Should().Contain("4");
        }

        [Fact]
        public void NumberOfClassesIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse", string.Empty)
                        )
                    )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfClasses());

            testRun.Results.First().Message.Should().Contain("1");
        }
    }
}