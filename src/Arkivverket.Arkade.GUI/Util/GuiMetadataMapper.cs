using System.Collections.Generic;
﻿using System;
using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.GUI.Models;

namespace Arkivverket.Arkade.GUI.Util
{
    internal static class GuiMetadataMapper
    {
        public static GuiMetaDataModel MapToArchiveDescription(string archiveDescription, string agreementNumber)
        {
            return new GuiMetaDataModel(archiveDescription ?? string.Empty, agreementNumber);
        }

        public static ObservableCollection<GuiMetaDataModel> MapToArchiveCreators(
            IEnumerable<MetadataEntityInformationUnit> archiveCreators)
        {
            return MapToGuiMetadataEntities(archiveCreators);
        }

        public static GuiMetaDataModel MapToTransferer(MetadataEntityInformationUnit transferrer)
        {
            return MapToGuiMetadataEntity(transferrer);
        }

        public static GuiMetaDataModel MapToProducer(MetadataEntityInformationUnit producer)
        {
            return MapToGuiMetadataEntity(producer);
        }

        public static ObservableCollection<GuiMetaDataModel> MapToOwners(
            IEnumerable<MetadataEntityInformationUnit> owners)
        {
            return MapToGuiMetadataEntities(owners);
        }

        public static GuiMetaDataModel MapToRecipient(string recipient)
        {
            return new GuiMetaDataModel(recipient, string.Empty, string.Empty, string.Empty);
        }

        public static GuiMetaDataModel MapToSystem(MetadataSystemInformationUnit system)
        {
            return new GuiMetaDataModel(system.Name, system.Version, system.Type, system.TypeVersion, GuiObjectType.system);
        }

        public static GuiMetaDataModel MapToArchiveSystem(MetadataSystemInformationUnit archiveSystem)
        {
            return MapToSystem(archiveSystem);
        }

        public static ObservableCollection<GuiMetaDataModel> MapToComments(IEnumerable<string> comments)
        {
            var guiMetadataComments = new ObservableCollection<GuiMetaDataModel>();

            foreach (string comment in comments)
                guiMetadataComments.Add(new GuiMetaDataModel(comment, GuiObjectType.comment));

            return guiMetadataComments;
        }

        public static GuiMetaDataModel MapToExtractionDate(DateTime? extractionDate)
        {
            return new GuiMetaDataModel(extractionDate);
        }

        private static ObservableCollectionEx<GuiMetaDataModel> MapToGuiMetadataEntities(
            IEnumerable<MetadataEntityInformationUnit> metadataEntityInformationUnits)
        {
            var guiMetadataEntities = new ObservableCollectionEx<GuiMetaDataModel>();

            foreach (MetadataEntityInformationUnit metadataEntityInformationUnit in metadataEntityInformationUnits)
                guiMetadataEntities.Add(MapToGuiMetadataEntity(metadataEntityInformationUnit));

            return guiMetadataEntities;
        }

        private static GuiMetaDataModel MapToGuiMetadataEntity(
            MetadataEntityInformationUnit metadataEntityInformationUnit)
        {
            return new GuiMetaDataModel(
                metadataEntityInformationUnit.Entity,
                metadataEntityInformationUnit.ContactPerson,
                metadataEntityInformationUnit.Telephone,
                metadataEntityInformationUnit.Email
            );
        }
    }
}
