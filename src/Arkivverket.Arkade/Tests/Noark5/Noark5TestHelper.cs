using System;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public static class Noark5TestHelper
    {
        public static bool IdentifiesJournalPostRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("journalpost");
        }

        public static bool IdentifiesCasefolder(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("mappe") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("saksmappe");
        }

        public static bool PeriodSeparationIsSharp(Archive archive)
        {
            var archiveExtraction = GetAddmlObject(ArkadeConstants.ArkivuttrekkXmlFileName, archive);

            dataObject archiveExtractionElement = archiveExtraction.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property periodProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "periode");

            property inboundSeparation = periodProperty.properties[0];
            property outboundSeparation = periodProperty.properties[1];

            return inboundSeparation.value.Equals("skarp") && outboundSeparation.value.Equals("skarp");
        }

        public static addml GetAddmlObject(string addmlXmlFileName, Archive archive)
        {
            string addmlXmlFile = archive.WorkingDirectory.Content().WithFile(addmlXmlFileName).FullName;

            try
            {
                return SerializeUtil.DeserializeFromFile<addml>(addmlXmlFile);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JournalHead GetJournalHead(string journalXmlFileName, Archive archive)
        {
            string journalXmlFile = archive.WorkingDirectory.Content().WithFile(journalXmlFileName).FullName;

            try
            {
                return JournalGuillotine.Behead(journalXmlFile);
            }
            catch
            {
                return null;
            }
        }

        public static bool TryParseArchiveDate(string dateStringFromArchive, out DateTime dateTime)
        {
            var acceptedFormats = new[] { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ" };

            return DateTime.TryParseExact(dateStringFromArchive, acceptedFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime);
        }
    }
}
