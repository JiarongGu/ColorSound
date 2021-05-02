using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ColorSound.Process.Helper
{
    public static class ColorHelper
    {
        public static Color Average(this IEnumerable<Color> colorSet)
        {
            var set = colorSet.CalibrateFilter();

            var red = set.Select(c => (int)c.R).Average();
            var green = set.Select(c => (int)c.G).Average();
            var blue = set.Select(c => (int)c.B).Average();
            var alpha = set.Select(c => (int)c.A).Average();

            return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
        }

        public static Color[] Average(this IEnumerable<Color[]> colorSet)
        {
            var total = colorSet.FirstOrDefault()?.Length ?? 0;

            if (total == 0)
                return new Color[0];

            var color = new Color[total];

            for (var i = 0; i < total; i++)
            {
                color[i] = colorSet.Select(c => c[i]).Average();
            }

            return color;
        }

        public static Color[] CalibrateFilter(this IEnumerable<Color> colorSet)
        {
            var factors = colorSet.Select(x =>
            {
                var set = new int[] { x.R, x.G, x.B };
                return new { color = x, factor = set.Max() - set.Min() };
            });

            var maxFactor = factors.Max(f => f.factor);

            var minFactor = maxFactor * 0.9;

            return factors.Where(f => f.factor >= minFactor).Select(f => f.color).ToArray();
        }

        public static Color[] Calibrate(this IEnumerable<Color> colorSet, double r = 1, double g = 1, double b = 1)
        {
            return colorSet.Select(color =>
            {
                if (color.B > 210)
                    return color;

                if (color.B - color.R > 0)
                    return Color.FromArgb(color.A, GetColorValue(color.R, r), GetColorValue(color.G, g), GetColorValue(color.B, b));

                if (color.B - color.R < 0)
                    return Color.FromArgb(color.A, GetColorValue(color.R, r * 1.5), GetColorValue(color.G, g / 1.2), GetColorValue(color.B, b / 2));

                return color;
            }
            ).ToArray();
        }

        private static int GetColorValue(int color, double factor)
        {
            var value = (int)(color * factor);
            if (value > 255) return 255;
            if (value < 0) return 0;
            return value;
        }

        public static Color[] GenerateDefaultColorArray(this int total)
        {
            return Enumerable.Range(0, total).Select(i => Color.FromArgb(0, 0, 0)).ToArray();
        }

        public static Windows.UI.Color GetUIColor(this Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
