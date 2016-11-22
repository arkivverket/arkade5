using Xunit;
using System.Globalization;
using System.Numerics;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions.DataTypes
{
    public class JosteinTest
    {

        [Fact]
        public void Test()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberGroupSeparator = ".";
            nfi.NumberGroupSizes = new int[] { 3 };

            BigInteger.Parse("1234.1234", NumberStyles.Integer | NumberStyles.AllowThousands, nfi);


            //BigInteger.Parse("4000E-2", NumberStyles.AllowExponent);
        }

    }
}
