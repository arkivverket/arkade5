using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public interface IRecordEnumerator : IEnumerator<Record>
    {
        public long RecordNumber { get; set; }
    }
}