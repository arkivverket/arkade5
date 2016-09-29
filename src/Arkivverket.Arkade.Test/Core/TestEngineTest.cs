using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestEngineTest
    {
        [Fact]
        public void ShouldEmitTestStartedEvent()
        {
            var testEngine = new TestEngine(CreateTestProviderMock(new DummyTest()));

            List<TestStartedEventArgs> events = new List<TestStartedEventArgs>();
            testEngine.TestStarted += delegate(object sender, TestStartedEventArgs args)
            {
                events.Add(args);
            };

            testEngine.RunTestsOnArchive(new TestSession(new Archive(Uuid.Of(""), "")));

            events.Count.Should().Be(1);
            events[0].TestName.Should().Be(typeof(DummyTest).FullName);
        }

        private static ITestProvider CreateTestProviderMock(DummyTest exampleTest)
        {
            var testProviderMock = new Mock<ITestProvider>();
            List<ITest> testList = new List<ITest>()
            {
                exampleTest
            };
            testProviderMock.Setup(t => t.GetTestsForArchive(It.IsAny<Archive>())).Returns(testList);
            return testProviderMock.Object;
        }
    }

    public class DummyTest : ITest
    {
        public TestRun RunTest(Archive archive)
        {
            return new TestRun(GetName(), TestType.Content);
        }

        public string GetName()
        {
            return GetType().FullName;
        }
    }
}
