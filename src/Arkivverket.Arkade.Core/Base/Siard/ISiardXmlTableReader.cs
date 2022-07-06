using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public interface ISiardXmlTableReader
    {
        IEnumerable<KeyValuePair<string, IEnumerable<byte>>> CreateLobByteArrays(string siardFileName);
    }
}
