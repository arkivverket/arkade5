namespace Arkivverket.Arkade.Core.Testing
{
    public class Location : ILocation
    {
        public string LocationString { get; }

        public Location(string locationString)
        {
            LocationString = locationString;
        }

        public static Location Archive => new Location(string.Empty);

        public override string ToString()
        {
            return LocationString;
        }
    }
}