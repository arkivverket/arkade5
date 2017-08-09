using System;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    internal class TimeDataType : DataType
    {
/* 
    <fieldType name="time"> 
    <dataType>time</dataType> 
    <fieldFormat>hhmmss</fieldFormat> 
    </fieldType> 
*/


        public override bool IsValid(string s)
        {
            throw new NotImplementedException();
        }
    }
}
