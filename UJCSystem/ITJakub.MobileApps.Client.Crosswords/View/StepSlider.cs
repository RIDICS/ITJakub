using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    [TemplatePart(Name = "InnerGrid", Type = typeof(Grid))]
    public sealed class StepSlider : Control
    {
        public StepSlider()
        {
            DefaultStyleKey = typeof(StepSlider);
            m_pointerState = PointerState.Released;
        }

        private Grid m_innerGrid;
        private PointerState m_pointerState;
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(int), typeof(StepSlider), new PropertyMetadata(default(int), OnStepChanged));
        public static readonly DependencyProperty StepWidthProperty = DependencyProperty.Register("StepWidth", typeof(double), typeof(StepSlider), new PropertyMetadata(20));
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof (Symbol), typeof (StepSlider), new PropertyMetadata(Symbol.Up));
        

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public double StepWidth
        {
            get { return (double)GetValue(StepWidthProperty); }
            set { SetValue(StepWidthProperty, value); }
        }

        public Symbol Symbol
        {
            get { return (Symbol) GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stepSlider = d as StepSlider;
            if (stepSlider == null)
                return;

            var newValue = (int) e.NewValue;
            
            stepSlider.m_innerGrid.Margin = new Thickness(newValue * stepSlider.StepWidth, 0, 0, 0);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_innerGrid = GetTemplateChild("InnerGrid") as Grid;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            var position = e.GetPosition(this);
            Step = (int) (position.X / StepWidth);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            m_pointerState = PointerState.Pressed;
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (m_pointerState != PointerState.Pressed)
                return;

            var position = e.GetCurrentPoint(this).Position;
            Step = (int)(position.X / StepWidth);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            m_pointerState = PointerState.Released;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            m_pointerState = PointerState.Released;
        }

        private enum PointerState
        {
            Released, Pressed
        }
    }
}