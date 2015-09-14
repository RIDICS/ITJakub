using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Fillwords.View.Converter;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.Fillwords.View.Control
{
    [TemplatePart(Name = "RichEditBox", Type = typeof(BindableRichEditBox))]
    [TemplatePart(Name = "Canvas", Type = typeof(Canvas))]
    public class RichEditComboBox : Windows.UI.Xaml.Controls.Control
    {
        private readonly List<ComboBoxItem> m_comboBoxList;
        private readonly AnswerStateToBackgroundConverter m_answerStateToBackgroundConverter;
        private BindableRichEditBox m_richEditBoxControl;
        private Canvas m_canvas;

        public RichEditComboBox()
        {
            DefaultStyleKey = typeof(RichEditComboBox);
            SizeChanged += OnSizeChanged;

            m_comboBoxList = new List<ComboBoxItem>();
            m_answerStateToBackgroundConverter = new AnswerStateToBackgroundConverter();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_richEditBoxControl = GetTemplateChild("RichEditBox") as BindableRichEditBox;
            m_canvas = GetTemplateChild("Canvas") as Canvas;
            LoadAndModifyDocument();
        }
        
        public string DocumentRtf
        {
            get { return (string)GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value); }
        }

        public ObservableCollection<OptionsViewModel> OptionsList
        {
            get { return (ObservableCollection<OptionsViewModel>) GetValue(OptionsListProperty); }
            set { SetValue(OptionsListProperty, value);}
        }

        public bool IsAnsweringAllowed
        {
            get { return (bool)GetValue(IsAnsweringAllowedProperty); }
            set { SetValue(IsAnsweringAllowedProperty, value); }
        }

        public static readonly DependencyProperty OptionsListProperty = DependencyProperty.Register("OptionsList",
            typeof(ObservableCollection<OptionsViewModel>), typeof(RichEditComboBox),
            new PropertyMetadata(null, OnDocumentOrOptionsChanged));

        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf",
            typeof(string), typeof(RichEditComboBox),
            new PropertyMetadata(string.Empty, OnDocumentOrOptionsChanged));

        public static readonly DependencyProperty IsAnsweringAllowedProperty =
            DependencyProperty.Register("IsAnsweringAllowed", typeof(bool), typeof(RichEditComboBox),
                new PropertyMetadata(true, OnIsAnsweringAllowedChanged));
        

        private static void OnIsAnsweringAllowedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as RichEditComboBox;
            if (richEditBox == null || e == null)
                return;

            var newValue = (bool) e.NewValue;
            foreach (var comboBoxItem in richEditBox.m_comboBoxList)
            {
                comboBoxItem.ComboBox.IsEnabled = newValue;
            }
        }
        
        private static void OnDocumentOrOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as RichEditComboBox;
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

            const char emDash = (char)0x2014;
            const int charCountEnlarge = 3;

            var oldIsReadonlyState = m_richEditBoxControl.IsReadOnly;
            m_richEditBoxControl.IsReadOnly = false;
            m_richEditBoxControl.Document.SetText(TextSetOptions.FormatRtf, DocumentRtf);
            var fulltTextRange = m_richEditBoxControl.Document.GetRange(0, m_richEditBoxControl.Document.Selection.StoryLength);
            fulltTextRange.ParagraphFormat.SetLineSpacing(LineSpacingRule.Multiple, 1.25f);

            m_comboBoxList.Clear();
            m_canvas.Children.Clear();
            int indexCorrection = 0;

            var sortedOptionsList = OptionsList.OrderBy(model => model.WordPosition);
            foreach (var optionsViewModel in sortedOptionsList)
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

                // create ComboBox
                var optionList = new List<string>(wordList) { optionsViewModel.CorrectAnswer };
                var comboBox = new OpaqueComboBox
                {
                    PlaceholderText = "???",
                    IsEnabled = IsAnsweringAllowed,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    ItemsSource = new ObservableCollection<string>(optionList.OrderBy(s => s))
                };
                
                // create ComboBox background binding
                var answerStateBinding = new Binding
                {
                    Path = new PropertyPath("AnswerState"),
                    Source = optionsViewModel,
                    Converter = m_answerStateToBackgroundConverter
                };
                var selectedAnswerBinding = new Binding
                {
                    Path = new PropertyPath("SelectedAnswer"),
                    Source = optionsViewModel,
                    Mode = BindingMode.TwoWay
                };
                comboBox.SetBinding(BackgroundProperty, answerStateBinding);
                comboBox.SetBinding(Selector.SelectedItemProperty, selectedAnswerBinding);

                // create ComboBoxItem
                var comboBoxItem = new ComboBoxItem
                {
                    Index = correctedWordPosition,
                    Length = maxWordLength,
                    ComboBox = comboBox,
                };
                
                m_comboBoxList.Add(comboBoxItem);
                m_canvas.Children.Add(comboBox);
                indexCorrection += maxWordLength - optionsViewModel.CorrectAnswer.Length;
            }
            
            UpdateComboBoxProperties();

            m_richEditBoxControl.IsReadOnly = oldIsReadonlyState;
        }

        private void UpdateComboBoxProperties()
        {
            const double comboBoxHeight = 26.0;
            foreach (var comboBoxItem in m_comboBoxList)
            {
                Rect rect;
                int hit;

                var textRange = m_richEditBoxControl.Document.GetRange(comboBoxItem.Index, comboBoxItem.Index + comboBoxItem.Length);
                textRange.GetRect(PointOptions.ClientCoordinates, out rect, out hit);

                if (rect.Width < 1)
                    return;

                // HACK scale values from RichEditBox
                var point = ScaleHelper.ScalePoint(false, rect.Left, rect.Bottom);
                var width = ScaleHelper.ScaleValue(false, rect.Width);

                var positionLeft = point.X + m_richEditBoxControl.Padding.Left + m_richEditBoxControl.BorderThickness.Left;
                var positionTop = point.Y - comboBoxHeight + 3.0;
                comboBoxItem.ComboBox.Margin = new Thickness(positionLeft, positionTop, 0, 0);
                comboBoxItem.ComboBox.Width = width; // For Windows 8.1 is suitable use correction -3, but Windows detection is requiered
            }
        }

        private class ComboBoxItem
        {
            public ComboBox ComboBox { get; set; }

            public int Index { get; set; }

            public int Length { get; set; }
        }
    }
}
