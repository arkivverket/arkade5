using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.GUI.Models;

namespace Arkivverket.Arkade.GUI.Util
{
    public static class ArchiveMetadataMapper
    {
        

        public static string MapToLabel(GuiMetaDataModel metaDataNoarkSection, bool standardLabelIsSelected)
        {
            return standardLabelIsSelected ? metaDataNoarkSection.StandardLabel : metaDataNoarkSection.UserdefinedLabel;
        }

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

        public static MetadataEntityInformationUnit MapToCreator(GuiMetaDataModel guiMetaDataModel)
        {
            return MapToMetadataEntityInformationUnit(guiMetaDataModel);
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

        public static DateTime? MapToStartDate(GuiMetaDataModel metaDataNoarkSection)
        {
            return metaDataNoarkSection.StartDate;
        }

        public static DateTime? MapToEndDate(GuiMetaDataModel metaDataNoarkSection)
        {
            return metaDataNoarkSection.EndDate;
        }

        public static DateTime? MapToExtractionDate(GuiMetaDataModel metaDataNoarkSection)
        {
            return metaDataNoarkSection.ExtractionDate;
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
                Address = guiMetaDataModel.Address,
                Telephone = guiMetaDataModel.Telephone,
                Email = guiMetaDataModel.Email
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

        internal static PackageType MapToPackageType(bool SelectedPackageTypeSip)
        {
            return SelectedPackageTypeSip 
                ? PackageType.SubmissionInformationPackage 
                : PackageType.ArchivalInformationPackage;
        }
    }
}
