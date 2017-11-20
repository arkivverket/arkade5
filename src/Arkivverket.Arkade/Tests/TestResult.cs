using System.Collections.Generic;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests
{
    public class TestResult
    {
        public ResultType Result { get; }
        public ILocation Location { get; }

        public string Message { get; }
        public int GroupErrors { get; }

        public TestResult(ResultType result, ILocation location, string message, int groupErrors = 0)
        {
            Assert.AssertNotNull("result", result);
            Assert.AssertNotNull("location", location);
            Assert.AssertNotNullOrEmpty("message", message);

            Result = result;
            Location = location;
            Message = message;
            GroupErrors = groupErrors;
        }
        
        public bool IsError()
        {
            return Result == ResultType.Error;
        }

        public bool IsErrorGroup()
        {
            return Result == ResultType.ErrorGroup;
        }

        public override string ToString()
        {
            return $"[{Result}] {Message}";
        }
    }

    public enum ResultType
    {
        Success,
        Error,
        ErrorGroup
    }
}