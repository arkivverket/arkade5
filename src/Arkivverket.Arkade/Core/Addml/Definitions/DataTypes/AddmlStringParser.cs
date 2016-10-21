
namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class AddmlStringParser
    {
        private readonly StringDataType _dataType;

        public AddmlStringParser(StringDataType dataType)
        {
            _dataType = dataType;
        }

        public string Parse(string s)
        {
            return s;
        }

    }
}
