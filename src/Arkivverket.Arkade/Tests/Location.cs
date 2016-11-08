namespace Arkivverket.Arkade.Tests
{
    public class Location : ILocation
    {
        public string Message { get; }

        public Location(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}