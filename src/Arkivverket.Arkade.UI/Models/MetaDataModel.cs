using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.Models
{
    public class MetaDataModel : BindableBase  
    {

        private string _archiveDescription;
        private string _agreementNumber;



        public string ArchiveDescription
        {
            get { return _archiveDescription; }
            set { SetProperty(ref _archiveDescription, value); }
        }
        public string AgreementNumber
        {
            get { return _agreementNumber; }
            set { SetProperty(ref _agreementNumber, value); }
        }


        public MetaDataModel()
        {
            
        }



    }
}
