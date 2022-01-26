using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Testing
{
    public interface ILocation
    {
        string FileName { get; }
        IEnumerable<long> ErrorLocations { get; }
        string ToString();
    }
}