using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Serilog;

namespace Arkivverket.Arkade.Core.Noark5
{
    public abstract class Noark5BaseTest : INoark5Test
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public abstract string GetName();
        public abstract TestType GetTestType();

        public void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
            _stopwatch.Start();
            ReadStartElementEvent(sender, e);
            _stopwatch.Stop();
        }

        public void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
            _stopwatch.Start();
            ReadEndElementEvent(sender, e);
            _stopwatch.Stop();
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs e)
        {
            _stopwatch.Start();
            ReadElementValueEvent(sender, e);
            _stopwatch.Stop();
        }

        public string GetDescription()
        {
            var description = Noark5TestDescriptions.ResourceManager.GetObject(GetName()) as string;

            if (description == null)
            {
                Log.Warning($"Missing description of Noark5Test: {GetType().FullName}");
            }

            return description;
        }

        public TestRun GetTestRun()
        {
            _stopwatch.Start();
            List<TestResult> testResults = GetTestResults();
            _stopwatch.Stop();

            return new TestRun(GetName(), GetTestType())
            {
                TestDescription = GetDescription(),
                Results = testResults,
                TestDuration = _stopwatch.ElapsedMilliseconds
            };
        }

        protected abstract List<TestResult> GetTestResults();
        protected abstract void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs);
    }
}