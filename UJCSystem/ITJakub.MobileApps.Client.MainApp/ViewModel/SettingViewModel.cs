using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
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
            ChangeThemeCommand = new RelayCommand<Color>(SetThemeColor);
        }
        
        public string Address { get; set; }

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand SetDefaultCommand { get; private set; }

        public RelayCommand<Color> ChangeThemeCommand { get; private set; }


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


        public void SetThemeColor(Color color)
        {
            Application.Current.Resources["TileBackgroundThemeBrush"] = new SolidColorBrush(color);
        }

        private void SetDefaultAddress()
        {
            m_dataService.UpdateEndpointAddress(null);
        }
    }
}