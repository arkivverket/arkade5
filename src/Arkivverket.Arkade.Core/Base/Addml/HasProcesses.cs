using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public interface HasProcesses
    {
        string GetName();

        List<string> GetProcesses();

    }
}
