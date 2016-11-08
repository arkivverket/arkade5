namespace Arkivverket.Arkade.Tests
{
    public class TestResult
    {
        public ResultType Result { get; }
        public ILocation Location { get; }

        public string Message { get; }

        public TestResult(ResultType result, ILocation location, string message)
        {
            Result = result;
            Location = location;
            Message = message;
        }

        public bool IsError()
        {
            return Result == ResultType.Error;
        }

        public override string ToString()
        {
            return $"[{Result}] {Message}";
        }
    }

    public enum ResultType
    {
        Success,
        Error
    }
}