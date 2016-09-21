namespace Arkivverket.Arkade.Tests
{
    public class TestResult
    {
        public ResultType Result { get; }

        public string Message { get; }

        public TestResult(ResultType result, string message)
        {
            Result = result;
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