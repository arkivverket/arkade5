using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersTest
    {
        [Fact]
        public void ForTwoArchivePartsWithOneSingleFolderThenNumberOfFoldersIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("mappe", string.Empty)
                        )
                    )
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("mappe", string.Empty)
                        )
                    )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFolders());

            testRun.Results.First().Message.Should().Contain("2");
        }

        [Fact]
        public void NumberOfFoldersIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper().Add("mappe", string.Empty)
                    )
                )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFolders());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void ShouldFindMultipleFoldersWithinSameArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper().Add("arkivdel",
                    new XmlElementHelper().Add("klassifikasjonssystem",
                        new XmlElementHelper()
                            .Add("mappe", string.Empty)
                            .Add("mappe", string.Empty)
                            .Add("mappe", string.Empty)
                    )
                )
            );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFolders());

            testRun.Results.First().Message.Should().Contain("3");
        }
    }
}