using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Testing.Siard
{
    public interface ISiardValidator
    {
        (List<string>, List<string>) Validate(string inputFilePath, string reportFilePath);
    }
}
