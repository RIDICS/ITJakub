using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Display;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public static class ScaleHelper
    {
        public static Point ScalePoint(bool inverse, double x, double y)
        {
            var resolutionScale = GetResolutionScale();

            if (resolutionScale == ResolutionScale.Scale100Percent)
                return new Point(x, y);

            var scale = (double)resolutionScale / 100.0;

            return inverse ? new Point(x / scale, y / scale) : new Point(x * scale, y * scale);
        }

        public static double ScaleValue(bool inverse, double value)
        {
            var resolutionScale = GetResolutionScale();
            if (resolutionScale == ResolutionScale.Scale100Percent)
                return value;

            var scale = (double)resolutionScale / 100.0;

            return inverse ? value / scale : value * scale;
        }

        private static ResolutionScale GetResolutionScale()
        {
            var resolutionScale = DesignMode.DesignModeEnabled
                ? ResolutionScale.Scale100Percent
                : DisplayInformation.GetForCurrentView().ResolutionScale;

            if (resolutionScale == ResolutionScale.Scale100Percent ||
                resolutionScale == ResolutionScale.Scale140Percent ||
                resolutionScale == ResolutionScale.Scale180Percent)
                return resolutionScale;

            // HACK for Windows 10, where following values are returned:
            //   116 for Scale140Percent
            //   149 for Scale180Percent
            // Windows Store App (Windows 8.1 target) using only these values:
            //   Scale100Percent, Scale140Percent, Scale180Percent

            if ((int) resolutionScale > 141)
                return ResolutionScale.Scale180Percent;

            if ((int) resolutionScale > 101)
                return ResolutionScale.Scale140Percent;

            return ResolutionScale.Scale100Percent;
        }
    }
}
