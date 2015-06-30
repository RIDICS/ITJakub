/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ITJakub.MobileApps.Client.Books"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using ITJakub.MobileApps.Client.Books.ViewModel.SelectPage;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public SelectBookViewModel SelectBookViewModel
        {
            get { return Container.Current.Resolve<SelectBookViewModel>(); }
        }

        public SelectPageViewModel SelectPageViewModel
        {
            get { return Container.Current.Resolve<SelectPageViewModel>(); }
        }
    }
}