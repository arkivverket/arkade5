using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public interface IArchiveFormatValidator : IDisposable
    {
        Task<ArchiveFormatValidationResult> ValidateAsync(FileSystemInfo item, ArchiveFormat format);
    }
}
