using ColorSound.Process.Helper;
using System.Drawing;
using System.Linq;

namespace ColorSound.Process
{

    public class ImagePixelData
    {
        public ImagePixelData(uint width, uint height, byte[,][] pixels, Color[,] pixelColors)
        {
            Height = height;
            Width = width;
            Pixels = pixels;
            PixelColors = pixelColors;
        }

        public byte[,][] Pixels { get; }

        public Color[,] PixelColors { get; }

        public uint Height { get; }

        public uint Width { get; }

        public Color[] GetFlattenColor(int widthFrom, int widthCount, int heightFrom, int heightCount)
        {
            var flattenColor = new Color[widthCount * heightCount];
            var count = 0;

            for (var h = 0; h < heightCount; h++)
            {
                for (var w = 0; w < widthCount; w++)
                {
                    flattenColor[count] = PixelColors[w + widthFrom, h + heightFrom];
                    count++;
                }
            }

            return flattenColor;
        }

        public Color[] GetReducedColor(int width, int height)
        {
            var total = Height / height;
            var color = new Color[total];

            for (var i = 0; i < total; i++)
            {
                var heightOffset = height * i;
                var flattenColor = GetFlattenColor((int)Width / 2, width, heightOffset, height);
                color[i] = flattenColor.Average();
            }

            return color;
        }
    }
}
