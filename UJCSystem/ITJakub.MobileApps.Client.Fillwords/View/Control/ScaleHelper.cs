using Windows.Foundation;
using Windows.Graphics.Display;

namespace ITJakub.MobileApps.Client.Fillwords.View.Control
{
    public static class ScaleHelper
    {
        public static Point ScalePoint(bool inverse, double x, double y)
        {
            var resolutionScale = DisplayInformation.GetForCurrentView().ResolutionScale;
            if (resolutionScale == ResolutionScale.Scale100Percent)
                return new Point(x, y);

            var scale = (double)resolutionScale / 100.0;

            return inverse ? new Point(x / scale, y / scale) : new Point(x * scale, y * scale);
        }

        public static double ScaleValue(bool inverse, double value)
        {
            var resolutionScale = DisplayInformation.GetForCurrentView().ResolutionScale;
            if (resolutionScale == ResolutionScale.Scale100Percent)
                return value;

            var scale = (double)resolutionScale / 100.0;

            return inverse ? value / scale : value * scale;
        }
    }
}
