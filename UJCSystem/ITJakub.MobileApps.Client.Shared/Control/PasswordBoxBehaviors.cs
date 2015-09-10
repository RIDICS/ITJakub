using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public class PasswordBoxBehaviors
    {
        public static readonly DependencyProperty EnterCommandProperty =
             DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand),
                 typeof(PasswordBoxBehaviors), new PropertyMetadata(null, OnEnterCommandPropertyChanged));

        public static void SetEnterCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(EnterCommandProperty, value);
        }

        public static ICommand GetEnterCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(EnterCommandProperty);
        }

        private static void OnEnterCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;

            if (passwordBox == null)
                return;

            if (e.NewValue == null)
            {
                passwordBox.KeyUp -= OnPasswordBoxKeyUp;
            }
            else
            {
                passwordBox.KeyUp += OnPasswordBoxKeyUp;
            }
        }

        private static void OnPasswordBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var enterCommand = GetEnterCommand(passwordBox);

            if (e.Key != VirtualKey.Enter || enterCommand == null)
                return;
            
            var passwordBindingExpression = passwordBox.GetBindingExpression(PasswordBox.PasswordProperty);
            if (passwordBindingExpression == null)
                return;

            passwordBindingExpression.UpdateSource();

            if (enterCommand.CanExecute(null))
                enterCommand.Execute(null);
        }
    }
}