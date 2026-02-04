using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reversi
{
    static class ColorBlending
    {
        public static Color Lerp(Color color, Color otherColor)
        {
            byte r = (byte)(0.5 * color.R + 0.5 * otherColor.R);
            byte g = (byte)(0.5 * color.G + 0.5 * otherColor.G);
            byte b = (byte)(0.5 * color.B + 0.5 * otherColor.B);

            return Color.FromRgb(r, g, b);
        }

        public static SolidColorBrush Lerp(this SolidColorBrush brush,
            SolidColorBrush otherBrush)
        {
            return new SolidColorBrush(Lerp(brush.Color, otherBrush.Color));
        }
    }
}
