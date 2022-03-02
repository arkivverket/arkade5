using System.Xml.Serialization;

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
        public string SchemaLocation = "http://www.loc.gov/METS/ https://schema.arkivverket.no/METS/latest/DIAS_METS.xsd";
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
