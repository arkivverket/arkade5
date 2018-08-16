using System;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public interface IArkadeTest : IComparable
    {
        /// <summary>
        ///     Returns the ID of the test
        /// </summary>
        /// <returns></returns>
        TestId GetId();

        /// <summary>
        ///     Returns the description of the test.
        /// </summary>
        /// <returns></returns>
        string GetDescription();

        /// <summary>
        /// Returns the test type: structure, content analysis or content control
        /// </summary>
        /// <returns></returns>
        TestType GetTestType();

        /// <summary>
        /// Returns the TestRun results.
        /// </summary>
        /// <returns></returns>
        TestRun GetTestRun();
    }
}
