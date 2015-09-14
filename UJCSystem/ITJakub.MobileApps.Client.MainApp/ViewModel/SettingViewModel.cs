using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;

        public SettingViewModel(IDataService dataService)
        {
            m_dataService = dataService;
            SaveCommand = new RelayCommand(Save);
            SetDefaultCommand = new RelayCommand(SetDefaultAddress);
        }
        
        public string Address { get; set; }

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand SetDefaultCommand { get; private set; }


        private bool IsAddressValid()
        {
            Uri uriResult;
            return Uri.TryCreate(Address, UriKind.Absolute, out uriResult);
        }

        private void Save()
        {
            if (!IsAddressValid())
                return;

            m_dataService.UpdateEndpointAddress(Address);
        }

        private void SetDefaultAddress()
        {
            m_dataService.UpdateEndpointAddress(null);
        }
    }
}