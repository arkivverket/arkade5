using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class Noark5TestProviderTests
    {
        private static Archive Archive =>
            TestUtil.CreateArchiveExtraction(Path.Combine("TestData", "Noark5", "Noark5Archive"));

        private static readonly List<TestId> AllTestIds = Noark5TestProvider.GetAvailableTests();

        private static readonly List<TestId> StructureTestIds = new List<TestId>
        {
            new TestId(TestId.TestKind.Noark5, 1),
            new TestId(TestId.TestKind.Noark5, 2),
            new TestId(TestId.TestKind.Noark5, 3),
            new TestId(TestId.TestKind.Noark5, 28)
        };

        private static readonly IEnumerable<TestId> ContentTestIds = AllTestIds.Except(StructureTestIds);

        private readonly Noark5TestProvider _noark5TestProvider = new Noark5TestProvider();


        [Fact]
        public void GetStructureTestsTest()
        {
            var testSession = new TestSession(Archive)
            {
                TestsToRun = AllTestIds
            };

            List<IArkadeStructureTest> returnedTests = _noark5TestProvider.GetStructureTests(testSession);

            IEnumerable<TestId> testIdsFromReturnedTests = returnedTests.Select(t => t.GetId());

            testIdsFromReturnedTests.Should().BeEquivalentTo(StructureTestIds);
        }

        [Fact]
        public void GetContentTestsTest()
        {
            var testSession = new TestSession(Archive)
            {
                TestsToRun = AllTestIds
            };

            List<INoark5Test> returnedTests = _noark5TestProvider.GetContentTests(testSession);

            IEnumerable<TestId> testIdsFromReturnedTests = returnedTests.Select(t => t.GetId());

            testIdsFromReturnedTests.Should().BeEquivalentTo(ContentTestIds);
        }

        [Fact]
        public void AllTestsAreReturned()
        {
            var testSession = new TestSession(Archive)
            {
                TestsToRun = AllTestIds
            };

            List<IArkadeStructureTest> returnedStructureTests = _noark5TestProvider.GetStructureTests(testSession);
            List<INoark5Test> returnedContentTests = _noark5TestProvider.GetContentTests(testSession);

            List<TestId> testIdsFromReturnedTests = returnedStructureTests.Select(t => t.GetId()).ToList();
            testIdsFromReturnedTests.AddRange(returnedContentTests.Select(t => t.GetId()));

            testIdsFromReturnedTests.Should().BeEquivalentTo(AllTestIds);
        }

        [Fact]
        public void ExactlySelectedTestsAreReturned()
        {
            var selectedTestsIds = new List<TestId>
            {
                new TestId(TestId.TestKind.Noark5, 1), // Structure test
                new TestId(TestId.TestKind.Noark5, 2), // Structure test
                new TestId(TestId.TestKind.Noark5, 15), // Content test
                new TestId(TestId.TestKind.Noark5, 22) // Content test
            };

            var testSession = new TestSession(Archive)
            {
                TestsToRun = selectedTestsIds
            };

            List<IArkadeStructureTest> returnedStructureTests = _noark5TestProvider.GetStructureTests(testSession);
            List<INoark5Test> returnedContentTests = _noark5TestProvider.GetContentTests(testSession);

            List<TestId> testIdsFromReturnedTests = returnedStructureTests.Select(t => t.GetId()).ToList();
            testIdsFromReturnedTests.AddRange(returnedContentTests.Select(t => t.GetId()));

            testIdsFromReturnedTests.Should().BeEquivalentTo(selectedTestsIds);
        }
    }
}
