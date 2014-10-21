using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
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
