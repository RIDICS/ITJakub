using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ITJakub.MobileApps.Client.Shared.Template;

namespace ITJakub.MobileApps.Client.Hangman.View.Control.HangmanPicture
{
    public class HangmanPictureTemplateSelector : DataTemplateSelector
    {
        private readonly Type[] m_hangmanTypeArray;
        private List<DataTemplate> m_hangmanDataTemplates;

        public HangmanPictureTemplateSelector()
        {
            m_hangmanTypeArray = new []
            {
                typeof(Hangman1),
                typeof(Hangman2),
                typeof(Hangman3),
                typeof(Hangman4),
                typeof(Hangman5),
                typeof(Hangman6),
                typeof(Hangman7),
            };
            
            LoadAllDataTemplates();
        }

        private void LoadAllDataTemplates()
        {
            var templateCreator = new DataTemplateCreator();
            m_hangmanDataTemplates = new List<DataTemplate>();
            foreach (var hangmanPictureType in m_hangmanTypeArray)
            {
                var dataTemplate = templateCreator.CreateDataTemplate(hangmanPictureType);
                m_hangmanDataTemplates.Add(dataTemplate);
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is int))
                return m_hangmanDataTemplates[0];

            var selectedPictureIndex = (int) item;
            if (selectedPictureIndex >= 0 && selectedPictureIndex < m_hangmanDataTemplates.Count)
                return m_hangmanDataTemplates[selectedPictureIndex];

            return m_hangmanDataTemplates[0];
        }
    }
}
