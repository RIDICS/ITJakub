using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.View
{
    [TemplatePart(Name = "RichEditBox", Type = typeof(BindableRichEditBox))]
    public sealed class ComboRichEditBox : Control
    {
        private BindableRichEditBox m_richEditBoxControl;

        public ComboRichEditBox()
        {
            DefaultStyleKey = typeof(ComboRichEditBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_richEditBoxControl = GetTemplateChild("RichEditBox") as BindableRichEditBox;
            OnOptionsListChanged(this, m_lastPropertyChangedEventArgs);
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
            get { return (List<ComboBoxData>)GetValue(OptionsListProperty); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data",
            typeof (List<ComboBoxData>), typeof (ComboRichEditBox), new PropertyMetadata(new List<ComboBoxData>()));

        public static readonly DependencyProperty OptionsListProperty = DependencyProperty.Register("OptionsList",
            typeof(List<OptionsViewModel>), typeof(ComboRichEditBox),
            new PropertyMetadata(null, OnOptionsListChanged));
        
        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf",
            typeof(string), typeof(ComboRichEditBox),
            new PropertyMetadata(string.Empty));

        private static DependencyPropertyChangedEventArgs m_lastPropertyChangedEventArgs;

        private static void OnOptionsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            m_lastPropertyChangedEventArgs = e;

            var richEditBox = d as ComboRichEditBox;
            if (richEditBox == null || e == null)
                return;

            var newList = e.NewValue as List<OptionsViewModel>;
            if (newList == null || richEditBox.m_richEditBoxControl == null)
                return;

            var newDataList = new List<ComboBoxData>(newList.Count + 1);
            foreach (var optionsViewModel in newList)
            {
                var dataItem = new ComboBoxData();
                dataItem.WordList.Add(optionsViewModel.CorrectAnswer);
                dataItem.WordList.AddRange(optionsViewModel.List.Select(model => model.Word));
                dataItem.WordList = dataItem.WordList.OrderBy(s => s).ToList();

                Point point;
                Rect rect;
                int hit;
                var textRange = richEditBox.m_richEditBoxControl.Document.GetRange(optionsViewModel.WordPosition, optionsViewModel.WordPosition);
                textRange.GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Top, PointOptions.ClientCoordinates, out point);
                textRange.GetRect(PointOptions.ClientCoordinates, out rect, out hit);
                // TODO get correct position (GetPoint and GetRect now returns incorrect values)

                dataItem.SetPosition(rect.Left, rect.Top);

                newDataList.Add(dataItem);
            }

            richEditBox.SetValue(DataProperty, newDataList);
        }

        //TODO if RichEditBox size change then rearrange ComboBox positions


        public class ComboBoxData
        {
            public ComboBoxData()
            {
                WordList = new List<string>();
                Position = new Thickness(0);
            }

            public void SetPosition(double x, double y)
            {
                Position = new Thickness(x, y, 0, 0);
            }

            public Thickness Position { get; set; }

            public List<string> WordList { get; set; }
        }
    }
}
