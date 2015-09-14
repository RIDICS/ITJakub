using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Hangman.View.Control.HangmanPicture
{
    public class HangmanPictureTemplateSelector : DataTemplateSelector
    {
        private List<Type> m_hangmanTypeList;

        public HangmanPictureTemplateSelector()
        {
            m_hangmanTypeList = new List<Type>
            {
                typeof(Hangman1)
            };
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {

            return base.SelectTemplateCore(item, container);
        }
    }
}
