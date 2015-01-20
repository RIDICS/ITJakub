using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.View.Control
{
    [TemplatePart(Name = "RichEditBox", Type = typeof(BindableRichEditBox))]
    public sealed class ComboRichEditBox : Windows.UI.Xaml.Controls.Control
    {
        private BindableRichEditBox m_richEditBoxControl;

        public ComboRichEditBox()
        {
            DefaultStyleKey = typeof(ComboRichEditBox);
            SizeChanged += OnSizeChanged;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_richEditBoxControl = GetTemplateChild("RichEditBox") as BindableRichEditBox;
            LoadAndModifyDocument();
        }
        
        public string DocumentRtf
        {
            get { return (string)GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value); }
        }

        public List<OptionsViewModel> OptionsList
        {
            get { return (List<OptionsViewModel>) GetValue(OptionsListProperty); }
            set { SetValue(OptionsListProperty, value);}
        }

        public List<ComboBoxData> Data
        {
            get { return (List<ComboBoxData>)GetValue(DataProperty); }
        }

        public ICommand AnswerChangedCommand
        {
            get { return (ICommand) GetValue(AnswerChangedCommandProperty); }
            set { SetValue(AnswerChangedCommandProperty, value); }
        }

        public bool IsAnsweringAllowed
        {
            get { return (bool) GetValue(IsAnsweringAllowedProperty); }
            set { SetValue(IsAnsweringAllowedProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data",
            typeof (List<ComboBoxData>), typeof (ComboRichEditBox), new PropertyMetadata(new List<ComboBoxData>()));

        public static readonly DependencyProperty OptionsListProperty = DependencyProperty.Register("OptionsList",
            typeof(List<OptionsViewModel>), typeof(ComboRichEditBox),
            new PropertyMetadata(null, OnDocumentOrOptionsChanged));
        
        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf",
            typeof(string), typeof(ComboRichEditBox),
            new PropertyMetadata(string.Empty, OnDocumentOrOptionsChanged));

        public static readonly DependencyProperty AnswerChangedCommandProperty =
            DependencyProperty.Register("AnswerChangedCommand", typeof (ICommand), typeof (ComboRichEditBox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsAnsweringAllowedProperty =
            DependencyProperty.Register("IsAnsweringAllowed", typeof (bool), typeof (ComboRichEditBox),
                new PropertyMetadata(true));

        private static void OnDocumentOrOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as ComboRichEditBox;
            if (richEditBox == null || e == null)
                return;

            richEditBox.LoadAndModifyDocument();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateComboBoxProperties();
        }

        private void LoadAndModifyDocument()
        {
            if (string.IsNullOrEmpty(DocumentRtf) || OptionsList == null || m_richEditBoxControl == null)
                return;

            const char emDash = (char) 0x2014;
            const int charCountEnlarge = 3;

            var oldIsReadonlyState = m_richEditBoxControl.IsReadOnly;
            m_richEditBoxControl.IsReadOnly = false;
            m_richEditBoxControl.Document.SetText(TextSetOptions.FormatRtf, DocumentRtf);

            var newDataList = new List<ComboBoxData>(OptionsList.Count + 1);
            int indexCorrection = 0;

            foreach (var optionsViewModel in OptionsList.OrderBy(model => model.WordPosition))
            {
                var wordList = optionsViewModel.List.Select(model => model.Word).ToList();
                int maxWordLength = wordList.Max(s => s.Length);
                if (maxWordLength < optionsViewModel.CorrectAnswer.Length)
                    maxWordLength = optionsViewModel.CorrectAnswer.Length;
                maxWordLength += charCountEnlarge;

                // create replacement for current word in the document
                var stringBuilder = new StringBuilder(maxWordLength, maxWordLength);
                stringBuilder.Append(emDash, maxWordLength);
                var characterSequence = stringBuilder.ToString();
                
                // select word for remove
                int correctedWordPosition = indexCorrection + optionsViewModel.WordPosition;
                var textRange = m_richEditBoxControl.Document.GetRange(correctedWordPosition, correctedWordPosition);
                textRange.MoveEnd(TextRangeUnit.Word, 1);
                while (Math.Abs(textRange.Length) > 0 && char.IsWhiteSpace(textRange.Text.Last()))
                {
                    textRange.MoveEnd(TextRangeUnit.Character, -1);
                }
                
                // replace selected word with character sequence
                textRange.SetText(TextSetOptions.None, characterSequence);
                textRange.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 1.5f);
                
                // create ComboBoxData
                var comboBoxData = new ComboBoxData(optionsViewModel, InvokeAnswerChangedCommand)
                {
                    Index = correctedWordPosition,
                    Length = maxWordLength
                };
                comboBoxData.WordList.Add(optionsViewModel.CorrectAnswer);
                comboBoxData.WordList.AddRange(wordList);
                comboBoxData.WordList = comboBoxData.WordList.OrderBy(s => s).ToList();

                newDataList.Add(comboBoxData);
                indexCorrection += maxWordLength - optionsViewModel.CorrectAnswer.Length;
            }

            SetValue(DataProperty, newDataList);
            UpdateComboBoxProperties();

            m_richEditBoxControl.IsReadOnly = oldIsReadonlyState;
        }

        private void UpdateComboBoxProperties()
        {
            foreach (var comboBoxData in Data)
            {
                Rect rect;
                int hit;
                
                var textRange = m_richEditBoxControl.Document.GetRange(comboBoxData.Index, comboBoxData.Index + comboBoxData.Length);
                textRange.GetRect(PointOptions.ClientCoordinates, out rect, out hit);

                comboBoxData.SetPosition(rect.Left + m_richEditBoxControl.Padding.Left + m_richEditBoxControl.BorderThickness.Left, rect.Top);
                comboBoxData.Width = rect.Width - 3;
            }
        }

        private void InvokeAnswerChangedCommand(OptionsViewModel optionsViewModel)
        {
            if (AnswerChangedCommand == null || !AnswerChangedCommand.CanExecute(optionsViewModel))
                return;
            
            AnswerChangedCommand.Execute(optionsViewModel);
        }

        public class ComboBoxData : ViewModelBase
        {
            private readonly OptionsViewModel m_optionsViewModel;
            private readonly Action<OptionsViewModel> m_answerChangedCallback;
            private Thickness m_position;
            private double m_width;
            private string m_selectedWord;

            public ComboBoxData(OptionsViewModel optionsViewModel, Action<OptionsViewModel> answerChangedCallback)
            {
                m_optionsViewModel = optionsViewModel;
                m_answerChangedCallback = answerChangedCallback;
                WordList = new List<string>();
                Position = new Thickness(0);
            }

            public void SetPosition(double x, double y)
            {
                Position = new Thickness(x, y, 0, 0);
            }

            public Thickness Position
            {
                get { return m_position; }
                set
                {
                    m_position = value;
                    RaisePropertyChanged();
                }
            }

            public double Width
            {
                get { return m_width; }
                set
                {
                    m_width = value;
                    RaisePropertyChanged();
                }
            }

            public int Length { get; set; }

            public int Index { get; set; }

            public List<string> WordList { get; set; }

            public OptionsViewModel OptionsViewModel
            {
                get { return m_optionsViewModel; }
            }

            public string SelectedWord
            {
                get { return m_selectedWord; }
                set
                {
                    m_selectedWord = value;
                    m_optionsViewModel.SelectedAnswer = value;
                    m_answerChangedCallback(m_optionsViewModel);
                }
            }
        }
    }
}
