using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class AddmlFloatParser
    {
        private readonly FloatDataType _dataType;

        public AddmlFloatParser(FloatDataType dataType)
        {
            Assert.AssertNotNull("dataType", dataType);

            _dataType = dataType;
        }

        public float Parse(String s)
        {
            // TODO: Implement!
            return float.Parse(s);
        }
    }
}