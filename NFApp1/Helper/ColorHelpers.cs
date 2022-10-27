using LuminInside.Enumerations;
using System;
using System.Collections;
using System.Threading;

namespace LuminInside.Helper
{
    public static class ColorHelpers
    {
        public static Color CheckColor(byte alpha, byte red, byte green, byte blue)
        {
            if (alpha < 0) alpha = 0;
            if (alpha > 255) alpha = 255;

            if (red < 0) red = 0;
            if (red > 255) red = 255;

            if (green < 0) green = 0;
            if (green > 255) green = 255;

            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;

            return Color.FromArgb(alpha, red, green, blue);
        }

        public static Color CheckColor(byte red, byte green, byte blue)
        {
            return CheckColor(255, red, green, blue);
        }


        public static IList ColorGradient(Color startColor, Color endColor, int size, ColorInterpolationMode interpolationMode = ColorInterpolationMode.HueMode)
        {
            if (startColor.R == endColor.R && startColor.G == endColor.G && startColor.B == endColor.B)
            {

                IList colors = new ArrayList();

                for (int i = 0; i < size; i++)
                {
                    colors.Add(startColor);
                }

                return colors;
               
            }

            switch (interpolationMode)
            {
                case ColorInterpolationMode.HueMode:
                    return CalculateGradientHUE(endColor, startColor, size);
                case ColorInterpolationMode.HueNearestMode:
                    throw new NotImplementedException();// return CalculateGradientHUENearest(endColor, startColor, size);
                case ColorInterpolationMode.Linear:
                    return CalculateGradientLinear(endColor, startColor, size);
                case ColorInterpolationMode.LinearCorrected:
                    return CalculateGradientLinearCorrected(endColor, startColor, size);
                default:
                    break;
            }

            return new ArrayList();
        }

        public static IList DimColor(Color color, int size, bool reverse = false)
        {
            HSLColor hslColor = color;

            double decrement = hslColor.Luminosity / ((double)size);
            double actualDecrement = 0;

            IList colors = new ArrayList();

            for (int i = 0; i < size; i++)
            {
                colors.Add(new HSLColor(hslColor.Hue, hslColor.Saturation, hslColor.Luminosity - actualDecrement));
                actualDecrement += decrement;
            }

            //ToDo: Implement a reverse function
            ////if (reverse)
            ////    colors.Reverse();

            return colors;
        }

        private static IList CalculateGradientLinear(Color endColor, Color startColor, int size)
        {
            byte rMax = startColor.R;
            int rMin = endColor.R;

            int gMax = startColor.G;
            int gMin = endColor.G;

            int bMax = startColor.B;
            int bMin = endColor.B;

            var colorList = new ArrayList();
            for (int i = 0; i < size; i++)
            {
                var rAverage = rMin + (int)((rMax - rMin) * i / size);
                var gAverage = gMin + (int)((gMax - gMin) * i / size);
                var bAverage = bMin + (int)((bMax - bMin) * i / size);
                colorList.Add(Color.FromArgb((byte)rAverage, (byte)gAverage, (byte)bAverage));
            }
            return colorList;
        }

        private static IList CalculateGradientLinearCorrected(Color startColor, Color endColor, int size)
        {
            IList colors = new ArrayList();

            int discreteUnits = size;
            float correctionFactor = 0.0f;
            float correctionFactorStep = 1.0f / discreteUnits;

            for (int i = 0; i < discreteUnits; i++)
            {
                correctionFactor += correctionFactorStep;
                float red = (endColor.R - startColor.R) * correctionFactor + startColor.R;
                float green = (endColor.G - startColor.G) * correctionFactor + startColor.G;
                float blue = (endColor.B - startColor.B) * correctionFactor + startColor.B;
                colors.Add(Color.FromArgb(startColor.A, (byte)red, (byte)green, (byte)blue));
            }
            return colors;
        }

        private static IList CalculateGradientHUE(Color startColor, Color endColor, int size)
        {

            IList colors = new ArrayList();
            HSLColor startHlsColor = startColor;
            HSLColor endHlsColor = endColor;
            int discreteUnits = size;

            for (int i = 0; i < discreteUnits; i++)
            {
                var hueAverage = endHlsColor.Hue + (int)((startHlsColor.Hue - endHlsColor.Hue) * i / size);
                var saturationAverage = endHlsColor.Saturation + (int)((startHlsColor.Saturation - endHlsColor.Saturation) * i / size);
                var luminosityAverage = endHlsColor.Luminosity + (int)((startHlsColor.Luminosity - endHlsColor.Luminosity) * i / size);
                colors.Add(new HSLColor(hueAverage, saturationAverage, luminosityAverage));
            }
            return colors;

        }

        ////private static IList CalculateGradientHUENearest(Color startColor, Color endColor, int size)
        ////{

        ////    bool considerNearest = true;
        ////    IList colors = new ArrayList();
        ////    HSLColor startHlsColor = startColor;
        ////    HSLColor endHlsColor = endColor;
        ////    int discreteUnits = size;

        ////    for (int i = 0; i < discreteUnits; i++)
        ////    {
        ////        double hueAverage;
        ////        double saturationAverage;
        ////        double luminosityAverage;

        ////        if (considerNearest)
        ////        {
        ////            var deltaDirection = CalculateDeltaAndDirection(startHlsColor.Hue, endHlsColor.Hue, size);
        ////            hueAverage = endHlsColor.Hue + deltaDirection.direction * (int)(deltaDirection.delta * i);

        ////            deltaDirection = CalculateDeltaAndDirection(startHlsColor.Saturation, endHlsColor.Saturation, size);
        ////            saturationAverage = endHlsColor.Saturation + deltaDirection.direction * (int)(deltaDirection.delta * i);

        ////            deltaDirection = CalculateDeltaAndDirection(startHlsColor.Luminosity, endHlsColor.Luminosity, size);
        ////            luminosityAverage = endHlsColor.Luminosity + deltaDirection.direction * (int)(deltaDirection.delta * i);
        ////        }
        ////        else
        ////        {
        ////            hueAverage = endHlsColor.Hue + (int)((startHlsColor.Hue - endHlsColor.Hue) * i / size);
        ////            saturationAverage = endHlsColor.Saturation + (int)((startHlsColor.Saturation - endHlsColor.Saturation) * i / size);
        ////            luminosityAverage = endHlsColor.Luminosity + (int)((startHlsColor.Luminosity - endHlsColor.Luminosity) * i / size);
        ////        }
        ////        colors.Add(new HSLColor(hueAverage, saturationAverage, luminosityAverage));
        ////    }
        ////    return colors;

        ////}

        ////private static (double delta, int direction) CalculateDeltaAndDirection(double start, double end, int size)
        ////{
        ////    int direction = 1;
        ////    if (start > end)
        ////        direction = -1;

        ////    double delta = Math.Abs((direction == 1 ? end - start : start - end) / size);

        ////    return (delta, direction);

        ////}

        public static string HexConverter(Color c)
        {
            return "#" + c.A.ToString("X2")  + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        }

        public static String RGBConverter(Color c)
        {
            return "ARGB(" + c.A.ToString() + "," + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }

        ////public static Color FromHex(string hexColor)
        ////{
        ////    int a = int.Parse(hexColor.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
        ////    int r = int.Parse(hexColor.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
        ////    int g = int.Parse(hexColor.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
        ////    int b = int.Parse(hexColor.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);
        ////    Convert

        ////    return Color.FromArgb(a, r, g, b);
        ////}

        ////private int HexToInt(string hex)
        ////{
        ////    uint32_t hex2int(char* hex)
        ////    {
        ////        int val = 0;
        ////        while (*hex)
        ////        {
        ////            // get current character then increment
        ////            uint8_t byte = *hex++;
        ////            // transform hex character to the 4bit equivalent number, using the ascii table indexes
        ////            if (byte >= '0' && byte <= '9') byte = byte - '0';
        ////            else if (byte >= 'a' && byte <= 'f') byte = byte - 'a' + 10;
        ////            else if (byte >= 'A' && byte <= 'F') byte = byte - 'A' + 10;
        ////            // shift 4 to make space for new digit, and add the 4 bits of the new digit 
        ////            val = (val << 4) | (byte & 0xF);
        ////        }
        ////        return val;
        ////    }

        ////}

        public static bool CompareColor(Color colorOne, Color colorTwo)
        {
            return colorOne.R == colorTwo.R && colorOne.G == colorTwo.G && colorOne.B == colorTwo.B && colorOne.A == colorTwo.A;
        }
    }
}