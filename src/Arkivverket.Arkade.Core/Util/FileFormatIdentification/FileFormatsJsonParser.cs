using System.Runtime.CompilerServices;
using System.Text.Json;
using Arkivverket.Arkade.Core.Util.Json;

[assembly:InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    internal static class FileFormatsJsonParser
    {
        public static ArchiveFileFormats ParseArchiveFileFormats()
        {
            string archiveFileFormatsAsJson = ResourceUtil.ReadResource(ArkadeConstants.ArchiveFileFormatsJsonResource);
            return JsonSerializer.Deserialize<ArchiveFileFormats>(archiveFileFormatsAsJson, new JsonSerializerOptions
            {
                Converters = { new JsonDateTimeConverter{ } }
            });
        }
    }
}
