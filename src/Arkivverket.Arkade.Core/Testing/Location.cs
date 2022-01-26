using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Testing
{
    public class Location : ILocation
    {
        private const int MaxAmountOfErrorLocationsToDisplay = 10;
        public string FileName { get; }
        public IEnumerable<long> ErrorLocations { get; }
        public string LocationString { get; }

        public Location(string locationString)
        {
            LocationString = locationString;
        }

        public Location(string fileName, long errorLocation)
        {
            FileName = fileName;
            ErrorLocations = new List<long> {errorLocation};
        }

        public Location(string fileName, IEnumerable<long> errorLocations)
        {
            FileName = fileName;
            ErrorLocations = new List<long>(errorLocations);
        }

        public static Location Archive => new Location(string.Empty);

        public override string ToString()
        {
            if (ErrorLocations == null)
                return LocationString;

            long errorLocationCount = ErrorLocations.Count();

            string errorLocationString;

            if (errorLocationCount <= MaxAmountOfErrorLocationsToDisplay)
                errorLocationString = ErrorLocations.Aggregate("", (s, i) => s + i + ", ").TrimEnd(',', ' ');
            
            else
                errorLocationString = ErrorLocations.Take(MaxAmountOfErrorLocationsToDisplay).Aggregate("", (s, i) => s + i + ", ") +
                                      string.Format(Messages.ControlDataFormatMessageExtension,
                                          errorLocationCount - MaxAmountOfErrorLocationsToDisplay);
            
            return string.Format(Messages.LineTraceMessage, FileName, errorLocationString);
        }
    }
}