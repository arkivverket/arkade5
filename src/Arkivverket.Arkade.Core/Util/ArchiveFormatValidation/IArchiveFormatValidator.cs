using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public interface IArchiveFormatValidator
    {
        Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format);
    }
}
