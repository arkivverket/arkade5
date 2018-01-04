using Arkivverket.Arkade.Core.Addml.Definitions;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml
{
    public class HardcodedProcessRunner
    {
        private List<IAddmlHardcodedProcess> _hardcodedProcesses;

        public HardcodedProcessRunner(AddmlDefinition addmlDefinition, Archive archive,
            List<TestResult> delimiterErrorTestResults = null)
        {
            _hardcodedProcesses = new List<IAddmlHardcodedProcess> {
                new ControlExtraOrMissingFiles(addmlDefinition, archive)
            };

            if (delimiterErrorTestResults != null && delimiterErrorTestResults.Any())
                _hardcodedProcesses.Add(new ControlRecordAndFieldDelimiters(delimiterErrorTestResults));
        }

        public List<TestRun> Run()
        {
            return _hardcodedProcesses.Select(p => p.GetTestRun()).ToList();
        }
    }
}