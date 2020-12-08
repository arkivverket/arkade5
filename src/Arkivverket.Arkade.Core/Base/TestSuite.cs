﻿using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestSuite
    {
        public IEnumerable<TestRun> TestRuns => _testRuns.ToList();

        private readonly SortedSet<TestRun> _testRuns;

        public TestSuite()
        {
            _testRuns = new SortedSet<TestRun>();
        }

        public void AddTestRun(TestRun testRun)
        {
            bool testRunWasAdded = _testRuns.Add(testRun);

            if (!testRunWasAdded)
            {
                throw new Exception(
                    string.Format(ExceptionMessages.AddTestRunToTestSuite, testRun.TestName, testRun.TestId)
                );
            }
        }

        public int FindNumberOfErrors()
        {
            int sum = 0;
            foreach (var testRun in TestRuns)
            {
                sum += testRun.FindNumberOfErrors();
            }
            return sum;
        }

    }
}