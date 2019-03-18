using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public static class ComputationHelper
    {
        private static readonly Dictionary<int, (Point startPoint, Point endPoint)> GradientPointsByAngle =
            new Dictionary<int, (Point startPoint, Point endPoint)>();

        // Make sure we don't go below zero
        public static double Clamp(double value, double minValue, double maxValue)
        {
            if (value < minValue)
            {
                return minValue;
            }

            if (value > maxValue)
            {
                return maxValue;
            }

            return value;
        }

        public static (Point startPoint, Point endPoint) RadiusGradientToPoints(int angle)
        {
            if (GradientPointsByAngle.TryGetValue(angle, out (Point startPoint, Point endPoint) points))
            {
                return points;
            }

            double anglePercent = angle / 360.0f;
            double a = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.75) / 2)), 2);
            double b = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.0) / 2)), 2);
            double c = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.25) / 2)), 2);
            double d = Math.Pow(Math.Sin(2 * Math.PI * ((anglePercent + 0.5) / 2)), 2);

            var result = (new Point(a, b), new Point(c, d));
            GradientPointsByAngle.Add(angle, result);

            return result;
        }
    }
}
