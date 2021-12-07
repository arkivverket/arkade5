using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Testing
{
    public class Location : ILocation
    {
        public string FileName { get; }
        public IEnumerable<int> ErrorLocations { get; }
        public string LocationString { get; }

        public Location(string locationString)
        {
            LocationString = locationString;
        }

        public Location(string fileName, int errorLocation)
        {
            FileName = fileName;
            ErrorLocations = new List<int> {errorLocation};
        }

        public Location(string fileName, IEnumerable<int> errorLocations)
        {
            FileName = fileName;
            ErrorLocations = new List<int>(errorLocations);
        }

        public static Location Archive => new Location(string.Empty);

        public override string ToString()
        {
            return ErrorLocations == null
                ? LocationString
                : string.Format(Messages.LineTraceMessage, FileName, ErrorLocations.Aggregate("", (s, i) => s + i + ", ").TrimEnd(',', ' '));
        }
    }
}