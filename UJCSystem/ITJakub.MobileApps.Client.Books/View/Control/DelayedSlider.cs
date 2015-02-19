using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class DelayedSlider : Slider
    {
        public static readonly DependencyProperty DelayedValueProperty = DependencyProperty.Register("DelayedValue", typeof (double), typeof (DelayedSlider), new PropertyMetadata(default(int)));
        private readonly DispatcherTimer m_delayTimer;

        public DelayedSlider()
        {
            m_delayTimer = new DispatcherTimer();
            m_delayTimer.Interval = new TimeSpan(0, 0, 0, 0, 700);
            m_delayTimer.Tick += DelayedValueUpdate;
        }
        
        public double DelayedValue
        {
            get { return (double) GetValue(DelayedValueProperty); }
            set { SetValue(DelayedValueProperty, value); }
        }
        
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            m_delayTimer.Stop();
            m_delayTimer.Start();
        }

        private void DelayedValueUpdate(object sender, object e)
        {
            m_delayTimer.Stop();
            DelayedValue = Value;
        }
    }
}
