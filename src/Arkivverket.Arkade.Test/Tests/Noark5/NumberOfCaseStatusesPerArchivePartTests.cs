using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfCaseStatusesPerArchivePartTests
    {
        [Fact]
        public void NumberOfCaseStatusesForAvsluttetIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                     new XmlElementHelper()
                     .Add("systemID", "arkivdel57d6608566c0b9.14601960")
                     .Add("tittel", "Test")
                     .Add("klassifikasjonssystem" , 
                            new XmlElementHelper().Add("klasse",
                              new XmlElementHelper().Add("mappe",
                                 new XmlElementHelper().Add("saksstatus", "Avsluttet")))))
              );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCaseStatusesPerArchivePart());
            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfCaseStatusesForAvsluttetIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                     new XmlElementHelper()
                     .Add("systemID", "arkivdel57d6608566c0b9.14601960")
                     .Add("tittel", "Test")
                     .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse",
                              new XmlElementHelper().Add("mappe",
                                 new XmlElementHelper().Add("saksstatus", "Avsluttet")
                                 )
                            .Add("mappe",
                                 new XmlElementHelper().Add("saksstatus", "Avsluttet"))
                             ))
                        )
              );


            TestRun testRun = helper.RunEventsOnTest(new NumberOfCaseStatusesPerArchivePart());
            testRun.Results.First().Message.Should().Contain("2");
        }

        [Fact]
        public void NumberOfCaseStatusesIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                     new XmlElementHelper()
                     .Add("systemID", "arkivdel57d6608566c0b9.14601960")
                     .Add("tittel", "Test")
                     .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse",
                              new XmlElementHelper().Add("mappe",
                                 new XmlElementHelper().Add("saksstatus", "Avsluttet")
                                 )
                            .Add("mappe",
                                 new XmlElementHelper().Add("saksstatus", "Utgår"))
                             ))
                        )
              );


            TestRun testRun = helper.RunEventsOnTest(new NumberOfCaseStatusesPerArchivePart());
            var x = testRun.Results;
            testRun.Results.Count().Should().Be(2);
        }

    }
}
