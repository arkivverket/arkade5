using System.Collections.Generic;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Testing
{
    public class AddmlGroupLocation : ILocation
    {
        private readonly List<FieldIndex> _indexes;

        public AddmlGroupLocation(List<FieldIndex> indexes)
        {
            _indexes = indexes;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (FieldIndex index in _indexes)
            {
                builder.AppendLine(AddmlLocation.FromFieldIndex(index).ToString());
            }
            return builder.ToString();
        }
    }
}