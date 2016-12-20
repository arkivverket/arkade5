using System;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassesInMainClassificationSystemWithoutSubClassesOrFoldersTest
    {
        [Fact]
        public void NumberOfClassesWithoutFolderIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse", 
                                new XmlElementHelper()
                                .Add("mappe", String.Empty)
                            )
                        )
                    )
            );

            RunTestWith(helper).Message.Should().Contain("0");
        }

        [Fact]
        public void NumberOfClassesWithoutSubClassIsZero()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse",
                                new XmlElementHelper()
                                .Add("klasse", String.Empty)
                            )
                        )
                    )
            );

            RunTestWith(helper).Message.Should().Contain("0");
        }

        [Fact]
        public void NumberOfClassesWithoutSubClassesIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper()
                                .Add("klasse", string.Empty)
                        )
                    )
            );

            RunTestWith(helper).Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfClassesWithoutSubClassIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse", string.Empty)
                            .Add("klasse",
                                    new XmlElementHelper()
                                    .Add("klasse", 
                                    new XmlElementHelper())
                                        .Add("mappe", string.Empty)
                                )
                            .Add("klasse", string.Empty)
                        )
                    )
            );

            RunTestWith(helper).Message.Should().Contain("2");
        }

        [Fact]
        public void NumberOfClassesWithoutSubClassIsTwoInPrimaryClassificationSystem()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper()
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse", string.Empty)
                            .Add("klasse", string.Empty)
                            )
                        .Add("klassifikasjonssystem",
                            new XmlElementHelper()
                            .Add("klasse", string.Empty)
                            .Add("klasse", string.Empty)
                        )
                    )
            );

            RunTestWith(helper).Message.Should().Contain("2");
        }

        private TestResult RunTestWith(XmlElementHelper helper)
        {
            TestRun testRun = helper.RunEventsOnTest(new NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders());
            return testRun.Results.First();
        }

    }
}
