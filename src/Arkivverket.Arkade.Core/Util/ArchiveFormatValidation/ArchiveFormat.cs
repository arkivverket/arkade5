using System.ComponentModel;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public enum ArchiveFormat
    {
        [Description("PDF/A")]
        PdfA,
        [Description("DIAS-SIP")]
        DiasSip,
        [Description("DIAS-AIP")]
        DiasAip,
        [Description("DIAS-SIP-NOARK5")]
        DiasSipN5,
        [Description("DIAS-AIP-NOARK5")]
        DiasAipN5,
        [Description("DIAS-SIP-SIARD")]
        DiasSipSiard,
        [Description("DIAS-AIP-SIARD")]
        DiasAipSiard,
    }
}
