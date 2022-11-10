using Arkivverket.Arkade.Core.Base.Siard;

namespace Arkivverket.Arkade.Core.Testing.Siard
{
    public interface ISiardValidator
    {
        SiardValidationReport Validate(string inputFilePath, string reportFilePath);
    }
}
