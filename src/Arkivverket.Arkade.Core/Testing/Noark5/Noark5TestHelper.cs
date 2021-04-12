using System;
using System.Linq;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public static class Noark5TestHelper
    {
        public static bool IdentifiesJournalPostRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("journalpost");
        }

        public static bool IdentifiesMeetingRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("moete");
        }

        public static bool IdentifiesRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type");
        }

        public static bool IdentifiesTypefolder(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("mappe") &&
                   eventArgs.Name.Equals("xsi:type");
        }

        public static bool IdentifiesCasefolder(ReadElementEventArgs eventArgs)
        {
            return IdentifiesTypefolder(eventArgs) &&
                   eventArgs.Value.Equals("saksmappe");
        }
        public static bool IdentifiesBaseRegistrationInRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("basisregistrering");
        }

        public static bool PeriodSeparationIsSharp(Archive archive)
        {
            bool inboundSeparationIsSharp;
            bool outboundSeparationIsSharp;

            addml archiveExtraction = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlXmlUnit.File);

            try
            {
                dataObject archiveExtractionElement = archiveExtraction.dataset[0].dataObjects.dataObject[0];
                property infoElement = archiveExtractionElement.properties[0];
                property additionalInfoElement = infoElement.properties[1];
                property periodProperty =
                    additionalInfoElement.properties.FirstOrDefault(p => p.name == "periode");

                property inboundSeparationProperty = periodProperty.properties[0];
                property outboundSeparationProperty = periodProperty.properties[1];

                inboundSeparationIsSharp = inboundSeparationProperty.value.Equals("skarpt");
                outboundSeparationIsSharp = outboundSeparationProperty.value.Equals("skarpt");
            }
            catch
            {
                string exceptionMessage = string.Format(Resources.ExceptionMessages.PeriodSeparationParseError, ArkadeConstants.ArkivuttrekkXmlFileName);

                throw new ArkadeException(exceptionMessage);
            }

            return inboundSeparationIsSharp && outboundSeparationIsSharp;
        }

        public static bool TryParseValidXmlDate(string dateStringFromArchive, out DateTime dateTime)
        {
            try
            {
                dateTime = SerializeUtil.DeserializeFromString<DateField>(
                        $"<?xml version=\"1.0\"?>\r\n" +
                        $"<dateTime>\r\n" +
                        $"\t<date>{dateStringFromArchive}</date>\r\n" +
                        $"</dateTime>")
                    .Date;
                return true;
            }
            catch
            {
                //If parsing is not successful, value is not valid. Hence the error is caught in N5.03.
                dateTime = default;
                return false;
            }
        }

        public static string StripNamespace(string value)
        {
            int index = value.IndexOf(":");
            if (index >= 0)
            {
                return value.Substring(index + 1);
            }
            return value;
        }
    }

    [XmlType("dateTime")]
    public class DateField
    {
        [XmlElement("date")]
        public DateTime Date { get; set; }
    }
}
