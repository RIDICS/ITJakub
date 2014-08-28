// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HangmanView
    {
        public HangmanView()
        {
            InitializeComponent();
        }

        private void LetterTextBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            var textBox = (TextBox) sender;

            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
                binding.UpdateSource();

            var command = GuessButton.Command;
            if (command != null && command.CanExecute(null))
                command.Execute(null);
        }
    }
}
