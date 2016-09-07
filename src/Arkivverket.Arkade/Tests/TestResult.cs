namespace Arkivverket.Arkade.Tests
{
    public class TestResult
    {
        public ResultType Result { get; private set; }

        public string Message { get; private set; }

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
        Success, Error
    }
}
