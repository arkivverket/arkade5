using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Testing
{
    public interface ILocation
    {
        string FileName { get; }
        IEnumerable<int> ErrorLocations { get; }
        string ToString();
    }
}