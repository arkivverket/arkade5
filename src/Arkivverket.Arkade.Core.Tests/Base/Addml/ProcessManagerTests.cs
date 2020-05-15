using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class ProcessManagerTests
    {
        [Fact]
        public void GetProcessInstanceByNameTest()
        {
            var flatFileDefinitions = new List<AddmlFlatFileDefinition>
            {
                new AddmlFlatFileDefinitionBuilder().WithProcesses(
                    new List<string>
                    {
                        "Analyse_CountRecords",
                        "Control_AllFixedLength",
                        "Invalid process name"
                    }
                ).Build()
            };

            List<IAddmlProcess> instantiatedProcesses = new ProcessManager(
                new AddmlDefinition(flatFileDefinitions, null)
            ).GetAllProcesses();

            instantiatedProcesses.Should().Contain(p => p.GetName().Equals("Analyse_CountRecords"));
            instantiatedProcesses.Should().Contain(p => p.GetName().Equals("Control_AllFixedLength"));
            instantiatedProcesses.Should().NotContain(p => p.GetName().Equals("Invalid process name"));
        }
    }
}
