using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.Models
{
    public class PopulateMetadataDataModels
    {
        public PopulateMetadataDataModels() { }

        public void DatafillArchiveDescription(ArchiveMetadata archiveMetadata, ObservableCollection<GuiMetaDataModel> metaDataModel)
        {
            metaDataModel.Add(new GuiMetaDataModel(string.Empty, string.Empty));
        }

        public void DatafillArchiveCreator(ArchiveMetadata archiveMetadata, ObservableCollection<GuiMetaDataModel> metaDataModel)
        {
            metaDataModel.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void DatafillArchiveRecipient(ArchiveMetadata archiveMetadata, ObservableCollection<GuiMetaDataModel> metaDataModel)
        {
            metaDataModel.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void DatafillArchiveSystem(ArchiveMetadata archiveMetadata, ObservableCollection<GuiMetaDataModel> metaDataModel)
        {
            metaDataModel.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, GuiObjectType.system));
        }

        public void DatafillArchiveEntity(List<MetadataEntityInformationUnit> metaDataEntityInformationUnits, ObservableCollection<GuiMetaDataModel> metaDataModel)
        {
            foreach (var entity in metaDataEntityInformationUnits)
            {
                metaDataModel.Add(new GuiMetaDataModel(entity.Entity, entity.ContactPerson, entity.Telephone, entity.Email));
            }

        }

    }
}
