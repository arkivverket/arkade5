using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatInfoFilesGenerator
    {
        void Generate(IEnumerable<IFileFormatInfo> fileFormatInfoSet, string relativePathRoot,
            string resultFileFullPath);
    }
}