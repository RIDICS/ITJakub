using IT_Jakub.Views.ApplicationStart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Page rootPage = null;

        WriteableBitmap wb;
        int oldX = -1;
        int oldY = -1;

        public MainPage()
        {
            this.InitializeComponent();
            int w = Convert.ToInt32(Math.Round(image.Width, 0));
            int h = Convert.ToInt32(Math.Round(image.Height, 0));
            wb = new WriteableBitmap(w, h);
            wb.Clear(Color.FromArgb(255, 255, 255, 255));
            image.Source = wb;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            rootPage = e.Parameter as Page;
            mainFrame.Navigate(typeof(ApplicationStart),this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            if (mainFrame.CanGoBack) {
                mainFrame.GoBack();
            } else if (rootPage != null && rootPage.Frame.CanGoBack) {
                rootPage.Frame.GoBack();
            }
        }

        /*
        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e) {
            PointerPoint o = e.GetCurrentPoint(canvas);
            Point p = new Point(o.Position.X,o.Position.Y);
            
            
        }
         *
         */
        

        private void MainButton_Click(object sender, RoutedEventArgs e) {
            mainFrame.Navigate(typeof(TestPage), this);
        }

        private void image_PointerPressed(object sender, PointerRoutedEventArgs e) {
            int x = (int)e.GetCurrentPoint(image).Position.X;
            int y = (int)e.GetCurrentPoint(image).Position.Y;
            wb.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
            oldX = x;
            oldY = y;
        }

        private void image_PointerReleased(object sender, PointerRoutedEventArgs e) {
            oldX = -1;
            oldY = -1;
        }

        private void image_PointerMoved(object sender, PointerRoutedEventArgs e) {
            if (e.GetCurrentPoint(image).Properties.IsLeftButtonPressed) {
                int x = (int)e.GetCurrentPoint(image).Position.X;
                int y = (int)e.GetCurrentPoint(image).Position.Y;
                wb.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                if (oldX != -1 || oldY != -1) {
                    wb.DrawLineAa(oldX, oldY, x, y, Color.FromArgb(255, 0, 0, 0));
                }
                oldX = x;
                oldY = y;
            }
        }
        
    }
}
