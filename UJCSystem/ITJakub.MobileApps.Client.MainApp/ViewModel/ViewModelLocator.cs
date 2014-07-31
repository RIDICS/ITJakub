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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Core;
using ITJakub.MobileApps.Client.Core.DataService;
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

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RegistrationViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<GroupListViewModel>();
            SimpleIoc.Default.Register<ApplicationHostViewModel>();
            SimpleIoc.Default.Register<ApplicationSelectionViewModel>();
            SimpleIoc.Default.Register<EditGroupViewModel>();
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public RegistrationViewModel RegistrationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<RegistrationViewModel>(); }
        }

        public ChatViewModel ChatViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ChatViewModel>(); }
        }

        public GroupListViewModel GroupListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<GroupListViewModel>(); }
        }

        public ApplicationHostViewModel ApplicationHostViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ApplicationHostViewModel>(); }
        }

        public ApplicationSelectionViewModel ApplicationSelectionViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ApplicationSelectionViewModel>(); }
        }

        public EditGroupViewModel EditGroupViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditGroupViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}