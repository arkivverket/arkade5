using System.Collections.Generic;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public interface ISiardXmlTableReader
    {
        List<IFileFormatInfo> GetFormatAnalysedLobs(string siardFileName);
    }
}
