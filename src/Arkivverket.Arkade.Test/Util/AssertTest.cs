using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class AssertTest
    {

        [Fact]
        public void AssertNotNullShouldThrowExeptionWhenParamenterIsNull()
        {
            var exception = Assert.Throws<ArkadeException>(() => Arkade.Util.Assert.AssertNotNull("variable", null));
            Assert.Equal("Finner ingen referanse til variable. Dette må være definert.", exception.Message);
        }

        [Fact]
        public void AssertNotNullShouldNotThrowExeptionWhenParamenterIsNotNull()
        {
            Arkade.Util.Assert.AssertNotNull("variable", "not null");
        }

    }
}
