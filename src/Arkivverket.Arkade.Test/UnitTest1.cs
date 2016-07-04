using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Add(2, 2).Should().Be(4);
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }


}
