using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using WinRTXamlToolkit.Controls.Extensions;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    // WinRT Implementation of the base Behavior classes
    public abstract class Behavior<T> : Behavior where T : DependencyObject
    {
        protected new T AssociatedObject
        {
            get { return base.AssociatedObject as T; }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject == null) throw new InvalidOperationException("AssociatedObject is not of the right type");
        }
    }

    public abstract class Behavior : DependencyObject, IBehavior
    {
        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            OnAttached();
        }

        public void Detach()
        {
            OnDetaching();
        }

        protected virtual void OnAttached()
        {

        }

        protected virtual void OnDetaching()
        {

        }

        protected DependencyObject AssociatedObject { get; set; }

        DependencyObject IBehavior.AssociatedObject
        {
            get { return this.AssociatedObject; }
        }
    }


    public class CloseFlyoutBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObjectOnClick;
        }

        private void AssociatedObjectOnClick(object sender, RoutedEventArgs routedEventArgs)
        {

            var element = AssociatedObject as DependencyObject;
            var flyout = element.GetFirstAncestorOfType<FlyoutPresenter>();

            if (flyout != null)
            {
                var popup = flyout.Parent as Popup;
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
        }
    }
}