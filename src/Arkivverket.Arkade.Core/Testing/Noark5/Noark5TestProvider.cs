using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class Noark5TestProvider : ITestProvider
    {
        private readonly List<TestId> _structureTests = new List<TestId>
        {
            new TestId(TestId.TestKind.Noark5, 1),
            new TestId(TestId.TestKind.Noark5, 2),
            new TestId(TestId.TestKind.Noark5, 3),
            new TestId(TestId.TestKind.Noark5, 28)
        };

        public List<IArkadeStructureTest> GetStructureTests(TestSession testSession)
        {
            var noark5TestFactory = new Noark5TestFactory(testSession.Archive);

            IEnumerable<TestId> testIds = testSession.TestsToRun.Intersect(_structureTests);

            var structureTests = new List<IArkadeStructureTest>();

            foreach (TestId testId in testIds)
            {
                var structureTest = (IArkadeStructureTest) noark5TestFactory.Create(testId);

                structureTests.Add(structureTest);
            }

            return structureTests;
        }

        public List<INoark5Test> GetContentTests(TestSession testSession)
        {
            var noark5TestFactory = new Noark5TestFactory(testSession.Archive);

            IEnumerable<TestId> testIds = testSession.TestsToRun.Except(_structureTests);

            var contentTests = new List<INoark5Test>();

            foreach (TestId testId in testIds)
            {
                var contentTest = (INoark5Test) noark5TestFactory.Create(testId);

                contentTests.Add(contentTest);
            }

            return contentTests;
        }

        public static List<TestId> GetAllTestIds()
        {
            return new Noark5TestFactory().GetTestIds();
        }

        public static SortedDictionary<TestId, TestType?> GetAvailableTests()
        {
            var availableTests = new SortedDictionary<TestId, TestType?>();

            foreach (TestId testId in GetAllTestIds())
                availableTests.Add(testId, Noark5TestFactory.GetTestType(testId));

            return availableTests;
        }
    }
}
