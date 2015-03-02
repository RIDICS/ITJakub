using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class ScrollViewerHelper
    {
        public static readonly DependencyProperty IsExternalZoomEnabledProperty = DependencyProperty.RegisterAttached("IsExternalZoomEnabled",
            typeof (bool), typeof(ScrollViewerHelper), new PropertyMetadata(false, OnZoomEnabledChanged));
        
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.RegisterAttached("Zoom",
            typeof (float), typeof (ScrollViewerHelper), new PropertyMetadata(1.0, OnZoomChanged));

        private static bool m_isViewChangeByScrollViewer;

        public static void SetIsExternalZoomEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsExternalZoomEnabledProperty, value);
        }

        public static bool GetIsExternalZoomEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(IsExternalZoomEnabledProperty);
        }

        public static void SetZoom(DependencyObject d, float value)
        {
            d.SetValue(ZoomProperty, value);
        }

        public static float GetZoom(DependencyObject d)
        {
            return (float)d.GetValue(ZoomProperty);
        }


        private static void OnZoomEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            var isEnabled = (bool) e.NewValue;
            if (isEnabled)
            {
                scrollViewer.ViewChanged += OnScrollOrZoom;
            }
            else
            {
                scrollViewer.ViewChanged -= OnScrollOrZoom;
            }
        }

        private static void OnScrollOrZoom(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null)
                return;

            m_isViewChangeByScrollViewer = true;

            if (!e.IsIntermediate && Math.Abs(GetZoom(scrollViewer) - scrollViewer.ZoomFactor) > 0.001)
                SetZoom(scrollViewer, scrollViewer.ZoomFactor);

            m_isViewChangeByScrollViewer = false;
        }

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            var newZoom = (float) e.NewValue;
            if (scrollViewer == null || Math.Abs(newZoom - scrollViewer.ZoomFactor) < 0.001)
                return;

            if (!m_isViewChangeByScrollViewer)
                scrollViewer.ChangeView(null, null, newZoom);
        }
    }
}
