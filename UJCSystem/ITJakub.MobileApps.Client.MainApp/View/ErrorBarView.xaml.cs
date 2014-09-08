﻿using Windows.UI.Core;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ITJakub.MobileApps.Client.MainApp.View
{
    public sealed partial class ErrorBarView
    {
        public ErrorBarView()
        {
            InitializeComponent();
            Window.Current.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            Width = e.Size.Width;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ToCollapsedStoryBoard.Begin();
            Window.Current.SizeChanged -= OnSizeChanged;
        }
    }
}