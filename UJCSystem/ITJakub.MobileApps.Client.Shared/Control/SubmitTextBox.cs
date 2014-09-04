using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public class SubmitTextBox : TextBox
    {
        public ICommand EnterCommand
        {
            get { return (ICommand) GetValue(EnterCommandProperty); }
            set { SetValue(EnterCommandProperty, value); }
        }

        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register("EnterCommand",
            typeof (ICommand), typeof (SubmitTextBox), new PropertyMetadata(null));

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key != VirtualKey.Enter || EnterCommand == null)
                return;

            var textBindingExpression = GetBindingExpression(TextProperty);
            if (textBindingExpression == null)
                return;
            
            textBindingExpression.UpdateSource();

            if (EnterCommand.CanExecute(null))
                EnterCommand.Execute(null);
        }
    }
}
