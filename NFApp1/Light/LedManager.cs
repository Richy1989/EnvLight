using System;
using System.Drawing;
using LuminInside.Common;
using LuminInside.Enumerations;
using LuminInside.Helper;
using NFApp1.Enumerations;
using NFApp1.Helper;

namespace NFApp1.Light
{
    public class LedManager
    {
        ILedController LedController { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public LedManager(ILedController ledController)
        {
            this.LedController = ledController;
        }

        public void LightOnOff(PowerOnOff onOff, LedSide side, Color onColor)
        {
            var leds = new LedScreen(LedController);

            for (int i = 0; i < LedController.LedCount; i++)
            {
                if (side == LedSide.Full)
                {
                    leds.SetPixel(ref i, onOff == PowerOnOff.On ? onColor : Color.Black);
                }
                else if (side == LedSide.Left && i < (int)Math.Round(LedController.LedCount / 2.0))
                {
                    leds.SetPixel(ref i, onOff == PowerOnOff.On ? onColor : Color.Black);
                }
                else if (side == LedSide.Right && i >= (int)Math.Round(LedController.LedCount / 2.0))
                {
                    leds.SetPixel(ref i, onOff == PowerOnOff.On ? onColor : Color.Black);
                }
                else
                {
                    if (LedController.ActualScreen != null)
                        leds.SetPixel(ref i, LedController.ActualScreen[i].LedColor);
                }
            }

            LedController.SendPixels(leds.pixels);
            StartColor = LedController.ActualScreen[0].LedColor;
            EndColor = LedController.ActualScreen[LedController.ActualScreen.Length - 1].LedColor;
        }

        public void SetRandomColor()
        {
            Random rnd = new();
            var hueStart = rnd.Next(360);
            HSLColor startHSL = new(((double)hueStart), ((double)100), ((double)(50)));

            rnd = new Random(300);
            var hueEnd = rnd.Next(360);
            HSLColor endHSL = new(((double)hueEnd), ((double)100), ((double)(50)));

            SetColor(startHSL, endHSL, ColorInterpolationMode.HueMode);
        }

        /// <summary>Sets the color.</summary>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="interpolationMode"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void SetColor(Color startColor, Color endColor, ColorInterpolationMode interpolationMode)
        {
            var leds = new LedScreen(LedController);

            StartColor = startColor;
            EndColor = endColor;

            var colors = ColorHelpers.ColorGradient(StartColor, EndColor, LedController.LedCount, interpolationMode);

            for (int i = 0; i < LedController.LedCount; i++)
            {
                leds.SetPixel(ref i, (Color)(colors[i]));
            }

            LedController.SendPixels(leds.pixels);
        }
    }
}
