/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ITJakub.MobileApps.MainApp"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using ITJakub.MobileApps.Client.MainApp.Common;
using ITJakub.MobileApps.Client.MainApp.Control.ViewModel;
using ITJakub.MobileApps.Client.MainApp.DataService;
using Microsoft.Practices.ServiceLocation;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
                // Create run time view services and models
                SimpleIoc.Default.Register<IDataService, DataService.DataService>();
            ////}

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<GroupListViewModel>();
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public ChatViewModel ChatViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ChatViewModel>(); }
        }

        public GroupListViewModel GroupListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GroupListViewModel>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}