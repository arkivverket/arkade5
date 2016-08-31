namespace Arkivverket.Arkade.Tests
{
    public class TestResult
    {
        public ResultType Result { get; private set; }

        public string TestName { get; private set; }

        public string Message { get; private set; }

        public TestResult(ResultType result, string testname, string message)
        {
            Result = result;
            TestName = testname;
            Message = message;
        }

        public bool IsError()
        {
            return Result == ResultType.Error;
        }
    }

    public enum ResultType
    {
        Success, Error
    }
}
