using System.ComponentModel;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public enum ArchiveFormat
    {
        [Description("PDF/A")]
        PdfA,
        [Description("DIAS SIP")]
        DiasSip,
        [Description("DIAS AIP")]
        DiasAip,
        [Description("DIAS SIP Noark5")]
        DiasSipN5,
        [Description("DIAS AIP Noark5")]
        DiasAipN5,
    }
}
