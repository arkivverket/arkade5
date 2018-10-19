using Arkivverket.Arkade.Core.Util;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class NorwegianNameGeneratorTest
    {
        public NorwegianNameGeneratorTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public void ShouldGenerateRandomNames()
        {
            for (int i = 0; i < 10; i++)
            {
                string randomName = NorwegianNameGenerator.RandomFirstNameAndLastName();
                //output.WriteLine(randomName);
            }
        }
    }
}