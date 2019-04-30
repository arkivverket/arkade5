using System;
using System.Globalization;
using System.Linq;
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

        public static bool TryParseArchiveDate(string dateStringFromArchive, out DateTime dateTime)
        {
            var acceptedFormats = new[]
            {
                "yyyy-MM-dd", // date only
                "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", // date + time [+ milliseconds] [+ timezone]
            };

            return DateTime.TryParseExact(dateStringFromArchive, acceptedFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime);
        }
    }
}
