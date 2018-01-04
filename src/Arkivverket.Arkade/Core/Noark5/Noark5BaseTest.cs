using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Serilog;

namespace Arkivverket.Arkade.Core.Noark5
{
    public abstract class Noark5BaseTest : IArkadeTest
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly Stopwatch Stopwatch = new Stopwatch();

        public abstract string GetName();
        public abstract TestType GetTestType();

        public string GetDescription()
        {
            var description = Noark5TestDescriptions.ResourceManager.GetObject(GetName()) as string;

            if (description == null)
            {
                Log.Debug($"Missing description of Noark5Test: {GetType().FullName}");
            }

            return description;
        }

        public TestRun GetTestRun()
        {
            Stopwatch.Start();
            List<TestResult> testResults = GetTestResults();
            Stopwatch.Stop();

            return new TestRun(this)
            {
                Results = testResults,
                TestDuration = Stopwatch.ElapsedMilliseconds
            };
        }

        protected abstract List<TestResult> GetTestResults();
    }
}
