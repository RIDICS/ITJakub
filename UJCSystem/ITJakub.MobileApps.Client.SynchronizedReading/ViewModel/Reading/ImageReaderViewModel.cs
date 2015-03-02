using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.SynchronizedReading.View.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.ViewModel.Reading
{
    public class ImageReaderViewModel : ViewModelBase
    {
        private double m_pointerPositionX;
        private double m_pointerPositionY;
        private ReaderImage.Modes m_currentMode;
        private ImageSource m_photo;
        private bool m_loading;

        public double PointerPositionX
        {
            get { return m_pointerPositionX; }
            set
            {
                m_pointerPositionX = value;
                RaisePropertyChanged();
            }
        }

        public double PointerPositionY
        {
            get { return m_pointerPositionY; }
            set
            {
                m_pointerPositionY = value;
                RaisePropertyChanged();
            }
        }

        public ReaderImage.Modes CurrentMode
        {
            get { return m_currentMode; }
            set
            {
                m_currentMode = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource Photo
        {
            get { return m_photo; }
            set
            {
                m_photo = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
            }
        }
    }
}