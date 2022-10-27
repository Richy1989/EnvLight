using LuminInside.Helper;

namespace LuminInside.Common
{
    public class LedPixel
    {
        public Color LedColor { get; set; } = Color.Black;
        public int Index { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
    }
}
