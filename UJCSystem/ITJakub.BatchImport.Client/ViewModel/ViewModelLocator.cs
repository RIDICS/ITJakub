/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ITJakub.BatchImport.Client"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Castle.MicroKernel.Registration;
using GalaSoft.MvvmLight;
using ITJakub.BatchImport.Client.DataService;

namespace ITJakub.BatchImport.Client.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Container.Current.Register(Classes.FromThisAssembly().BasedOn<ViewModelBase>().LifestyleTransient().WithServiceSelf().WithServiceBase());

            if (ViewModelBase.IsInDesignModeStatic)
            {
                Container.Current.Register(Component.For<IDataService>().ImplementedBy<DesignDataService>());
            }
            else
            {
                Container.Current.Register(Component.For<IDataService>().ImplementedBy<DataService.DataService>());
            }
        }

        public MainViewModel Main
        {
            get { return Container.Current.Resolve<MainViewModel>(); }
        }
    }
}