using System.Xml.Serialization;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;

namespace Arkivverket.Arkade.Core.ExternalModels.DiasMets
{
    // ReSharper disable once InconsistentNaming
    // This class extends the mets class generated from DIAS_METS.xsd. It adds the schemaLocation attribute to the mets class. 
    // This attribute will be appended to the output when the serializer is creating new mets files.
#pragma warning disable IDE1006
    public partial class mets
#pragma warning restore IDE1006
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation = "http://arkivverket.no/standarder/METS https://schema.arkivverket.no/METS/latest/DIAS_METS.xsd";
    }

    public enum AltRecordIdType
    {
        DELIVERYSPECIFICATION = metsTypeMetsHdrAltRecordIDTYPE.DELIVERYSPECIFICATION,
        SUBMISSIONAGREEMENT = metsTypeMetsHdrAltRecordIDTYPE.SUBMISSIONAGREEMENT,
        DELIVERYTYPE = metsTypeMetsHdrAltRecordIDTYPE.DELIVERYTYPE,
        PACKAGENUMBER = metsTypeMetsHdrAltRecordIDTYPE.PACKAGENUMBER,
        REFERENCECODE = metsTypeMetsHdrAltRecordIDTYPE.REFERENCECODE,
        STARTDATE = metsTypeMetsHdrAltRecordIDTYPE.STARTDATE,
        ENDDATE = metsTypeMetsHdrAltRecordIDTYPE.ENDDATE,
        PROJECTNAME = 999,
    }

    public enum RecordStatusType
    {
        NEW = metsTypeMetsHdrRECORDSTATUS.NEW,
        SUPPLEMENT = metsTypeMetsHdrRECORDSTATUS.SUPPLEMENT,
        REPLACEMENT = metsTypeMetsHdrRECORDSTATUS.REPLACEMENT,
        VERSION = metsTypeMetsHdrRECORDSTATUS.VERSION,
        TEST = metsTypeMetsHdrRECORDSTATUS.TEST,
        OTHER = metsTypeMetsHdrRECORDSTATUS.OTHER,
    }

    public enum MetsHdrAgentOtherRoleType
    {
        SUBMITTER = metsTypeMetsHdrAgentOTHERROLE.SUBMITTER,
        PRODUCER = metsTypeMetsHdrAgentOTHERROLE.PRODUCER,
    }
}

namespace Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription
{
    // ReSharper disable once InconsistentNaming
    // This class extends the mets class generated from submissionDescription.xsd. It adds the schemaLocation attribute to the mets class. 
    // This attribute will be appended to the output when the serializer is creating new mets files.
#pragma warning disable IDE1006
    public partial class mets
#pragma warning restore IDE1006
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation = "http://www.loc.gov/METS/ https://schema.arkivverket.no/INFOFIL/latest/submissionDescription.xsd";
    }
}
