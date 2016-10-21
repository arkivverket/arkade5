using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class AddmlIntegerParser
    {
        private readonly IntegerDataType _dataType;

        public AddmlIntegerParser(IntegerDataType dataType)
        {
            Assert.AssertNotNull("dataType", dataType);

            _dataType = dataType;
        }

        public int Parse(String s)
        {
            s = RemoveWhitespace(s);
            s = RemoveThousandSign(s);

            try
            {
                BigInteger bigInteger = BigInteger.Parse(s, NumberStyles.AllowExponent
                                                            | NumberStyles.AllowLeadingSign);

                // TODO: Possible overflow here!
                return (int) bigInteger;
            }
            catch (FormatException e)
            {
                throw new ArgumentException("Could not parse integer " + s);
            }
        }

        private string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        private string RemoveThousandSign(string s)
        {
            return s.Replace(".", "");
        }
    }
}