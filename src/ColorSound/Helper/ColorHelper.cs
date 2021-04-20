using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorSound.Helper
{
    public static class ColorHelper
    {
        public static Color Average(this IEnumerable<Color> colorSet) 
        {
            var red = colorSet.Select(c => (int)c.R).Average();
            var green = colorSet.Select(c => (int)c.G).Average();
            var blue = colorSet.Select(c => (int)c.B).Average();
            var alpha = colorSet.Select(c => (int)c.A).Average();

            return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
        }

        public static Color[] Average(this IEnumerable<Color[]> colorSet)
        {
            var total = colorSet.FirstOrDefault()?.Length ?? 0;

            if (total == 0)
                return new Color[0];

            var color = new Color[total];

            for (var i = 0; i < total; i++) {
                color[i] = colorSet.Select(c => c[i]).Average();
            }

            return color;
        }
    }
}
