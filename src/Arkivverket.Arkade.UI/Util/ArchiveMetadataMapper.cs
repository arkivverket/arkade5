using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Models;

namespace Arkivverket.Arkade.UI.Util
{
    public static class ArchiveMetadataMapper
    {
        public static string MapToArchiveDescription(GuiMetaDataModel guiMetaDataModel)
        {
            return guiMetaDataModel.ArchiveDescription;
        }

        public static string MapToAgreementNumber(GuiMetaDataModel guiMetaDataModel)
        {
            return guiMetaDataModel.AgreementNumber;
        }

        public static List<MetadataEntityInformationUnit> MapToArchiveCreators(
            IEnumerable<GuiMetaDataModel> guiMetaDataModels)
        {
            return MapToMetadataEntityInformationUnits(guiMetaDataModels);
        }
        
        public static MetadataEntityInformationUnit MapToTransferer(GuiMetaDataModel guiMetaDataModel)
        {
            return MapToMetadataEntityInformationUnit(guiMetaDataModel);
        }

        public static MetadataEntityInformationUnit MapToProducer(GuiMetaDataModel guiMetaDataModel)
        {
            return MapToMetadataEntityInformationUnit(guiMetaDataModel);
        }

        public static List<MetadataEntityInformationUnit> MapToArchiveOwners(
            IEnumerable<GuiMetaDataModel> guiMetaDataModels)
        {
            return MapToMetadataEntityInformationUnits(guiMetaDataModels);
        }

        public static string MapToRecipient(GuiMetaDataModel guiMetaDataModel)
        {
            return guiMetaDataModel.Entity;
        }

        public static MetadataSystemInformationUnit MapToSystem(GuiMetaDataModel guiMetaDataModel)
        {
            return MapToMetadataSystemInformationUnit(guiMetaDataModel);
        }

        public static MetadataSystemInformationUnit MapToArchiveSystem(GuiMetaDataModel guiMetaDataModel)
        {
            return MapToMetadataSystemInformationUnit(guiMetaDataModel);
        }

        public static List<string> MapToComments(IEnumerable<GuiMetaDataModel> guiMetaDataModels)
        {
            var archiveMetadataComments = new List<string>();

            foreach (GuiMetaDataModel guiMetaDataModel in guiMetaDataModels)
                archiveMetadataComments.Add(guiMetaDataModel.Comment);

            return archiveMetadataComments;
        }

        private static List<MetadataEntityInformationUnit> MapToMetadataEntityInformationUnits(
            IEnumerable<GuiMetaDataModel> guiMetaDataModels)
        {
            var metadataEntityInformationUnits = new List<MetadataEntityInformationUnit>();

            foreach (GuiMetaDataModel guiMetaDataModel in guiMetaDataModels)
                metadataEntityInformationUnits.Add(MapToMetadataEntityInformationUnit(guiMetaDataModel));

            return metadataEntityInformationUnits;
        }

        private static MetadataEntityInformationUnit MapToMetadataEntityInformationUnit(
            GuiMetaDataModel guiMetaDataModel)
        {
            return new MetadataEntityInformationUnit
            {
                Entity = guiMetaDataModel.Entity,
                ContactPerson = guiMetaDataModel.ContactPerson,
                Email = guiMetaDataModel.Email,
                Telephone = guiMetaDataModel.Telephone
            };
        }

        private static MetadataSystemInformationUnit MapToMetadataSystemInformationUnit(
            GuiMetaDataModel guiMetaDataModel)
        {
            return new MetadataSystemInformationUnit
            {
                Name = guiMetaDataModel.SystemName,
                Type = guiMetaDataModel.SystemType,
                Version = guiMetaDataModel.SystemVersion,
                TypeVersion = guiMetaDataModel.SystemTypeVersion
            };
        }
    }
}
