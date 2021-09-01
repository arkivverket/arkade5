using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base
{
    public interface IArchiveDetails
    {
        string ArchiveCreators { get; }
        string ArchivalPeriod { get; }
        string SystemName { get; }
        string SystemType { get; }
        string ArchiveStandard { get; }
        Dictionary<string, IEnumerable<string>> DocumentedXmlUnits { get; }
        Dictionary<string, IEnumerable<string>> StandardXmlUnits { get; }
    }
}