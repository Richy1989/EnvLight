using LuminInside.Common;
using LuminInside.Enumerations;
using LuminInside.Helper;
using NFApp1.Enumerations;
using System;

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
            Color startColor = Color.FromArgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));

            rnd = new Random();
            Color endColor = Color.FromArgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));

            SetColor(startColor, endColor, ColorInterpolationMode.HueMode);
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

            //////If mode is running, let only the color object change, but do not transfer colors to screen.
            ////if (IsModeRunning)
            ////{
            ////    var enumType = typeof(LedMode);
            ////    var memberInfos = enumType.GetMember(runningLedMode.ToString());
            ////    var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            ////    var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(LedModeAttribute), false);

            ////    if (!((LedModeAttribute)valueAttributes[0]).CanSetColor)
            ////        return;

            ////    useSmoothing = ((LedModeAttribute)valueAttributes[0]).UseSmoothing;
            ////}

            var colors = ColorHelpers.ColorGradient(StartColor, EndColor, LedController.LedCount, interpolationMode);

            for (int i = 0; i < LedController.LedCount; i++)
            {
                leds.SetPixel(ref i, (Color)colors[i]);
            }

            LedController.SendPixels(leds.pixels);
        }
    }
}
