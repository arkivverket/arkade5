using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Addml
{
    public class HardcodedProcessRunner
    {
        private List<IAddmlHardcodedProcess> _hardcodedProcesses;

        public HardcodedProcessRunner(AddmlDefinition addmlDefinition, Archive archive)
        {
            _hardcodedProcesses = new List<IAddmlHardcodedProcess> {
                new ControlExtraOrMissingFiles(addmlDefinition, archive)
            };
        }

        public List<TestRun> Run()
        {
            return _hardcodedProcesses.Select(p => p.GetTestRun()).ToList();
        }
    }
}