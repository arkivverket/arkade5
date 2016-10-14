using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class AssertTest
    {

        [Fact]
        public void AssertNotNullShouldThrowExeptionWhenParamenterIsNull()
        {
            var exception = Assert.Throws<Exception>(() => Arkade.Util.Assert.AssertNotNull("variable", null));
            Assert.Equal("variable cannot be null", exception.Message);
        }

        [Fact]
        public void AssertNotNullShouldNotThrowExeptionWhenParamenterIsNotNull()
        {
            Arkade.Util.Assert.AssertNotNull("variable", "not null");
        }

    }
}
