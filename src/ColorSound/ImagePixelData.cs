using System.Drawing;

namespace ColorSound
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

        public Color[] GetFlattenColor(int widthFrom, int widthTo, int heightFrom, int heightTo)
        {
            var flattenColor = new Color[(widthTo - widthFrom) * (heightTo - heightFrom)];
            var count = 0;

            for (var h = heightFrom; h < heightTo; h++)
            {
                for (var w = widthFrom; w < widthTo; w++)
                {
                    flattenColor[count] = PixelColors[w, h];
                    count++;
                }
            }

            return flattenColor;
        }
    }
}
