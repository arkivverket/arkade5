namespace Arkivverket.Arkade.Tests
{
    public class Location : ILocation
    {
        public string LocationString { get; }

        public Location(string locationString)
        {
            LocationString = locationString;
        }

        public override string ToString()
        {
            return LocationString;
        }
    }
}