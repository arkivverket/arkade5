using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace Arkivverket.Arkade.Core.ExternalModels.Mets
{
    // ReSharper disable once InconsistentNaming
    // This class extends the mets class generated from an xsd. It adds the schemalocation attribute to the mets class. 
    // This attribute will be appended to the output when the serializer is creating new mets files.
    public partial class mets
    {
        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation = "http://www.loc.gov/METS/ http://schema.arkivverket.no/METS/mets.xsd";
    }
}