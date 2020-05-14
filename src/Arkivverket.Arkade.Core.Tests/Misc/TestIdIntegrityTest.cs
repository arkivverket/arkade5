using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Misc
{
    public class ArkadeTestIntegrityControl
    {
        [Fact]
        public void AllTestsAndProcessesHasAUniqueTestId()
        {
            IEnumerable<TestId> allTestIds = Noark5TestProvider.GetAllTestIds()
                .Concat(ProcessProvider.GetAllProcesses().Select(p => p.GetId()));

            allTestIds.Should().OnlyHaveUniqueItems();
        }

        // TODO: Check that tests/processes are sorted
    }
}
