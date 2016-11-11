using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Logging;
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

            var statusEventHandler = new StatusEventHandler();

            var testEngine = new TestEngine(CreateTestProviderMock(new DummyTest()), statusEventHandler);

            List<OperationMessageEventArgs> events = new List<OperationMessageEventArgs>();
            statusEventHandler.TestStartedEvent += delegate(object sender,  OperationMessageEventArgs args)
            {
                events.Add(args);
            };

            testEngine.RunTestsOnArchive(new TestSessionBuilder().Build());

            events.Count.Should().Be(1);
            events[0].Id.Should().Be(typeof(DummyTest).FullName);
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
