using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public interface IArchiveFormatValidator : IDisposable
    {
        Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath="");
    }
}
