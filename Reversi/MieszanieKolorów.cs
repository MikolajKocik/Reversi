using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reversi
{
    static class MieszanieKolorów
    {
        public static Color Lerp(Color kolor, Color innyKolor)
        {
            byte r = (byte)(0.5 * kolor.R + 0.5 * innyKolor.R);
            byte g = (byte)(0.5 * kolor.G + 0.5 * innyKolor.G);
            byte b = (byte)(0.5 * kolor.B + 0.5 * innyKolor.B);

            return Color.FromRgb(r, g, b);
        }

        public static SolidColorBrush Lerp(this SolidColorBrush pędzel,
            SolidColorBrush innyPędzel)
        {
            return new SolidColorBrush(Lerp(pędzel.Color, innyPędzel.Color));
        }
    }
}
