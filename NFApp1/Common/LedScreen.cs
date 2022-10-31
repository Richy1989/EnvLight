using LuminInside.Helper;
using System;
using System.Drawing;

namespace LuminInside.Common
{ 
    public class LedScreen
    {
        public LedPixel[] pixels;
        private readonly ILedController controller;
        
        public LedScreen(ILedController controller)
        {
            this.controller = controller;

            if (controller == null)
                throw new Exception("Led Controller must not be Null in LedScreen");

            pixels = new LedPixel[controller.LedCount];
        }

        public void SetPixel(ref int index, Color color)
        {
            if (CheckIndexOverflow(ref index)) return;

            if (pixels[index] == null)
                pixels[index] = new LedPixel();

            pixels[index].LedColor = color;
            pixels[index].Index = index;
        }

        public void SetColorToScreen(Color color)
        {
            for (int i = 0; i < controller.LedCount; i++)
            {
                pixels[i] = new LedPixel
                {
                    LedColor = color
                };
            }
        }

        private bool CheckIndexOverflow(ref int index)
        {
            if (index >= controller.LedCount)
                return true;

            return false;
            //index = controller.LedCount - index;
        }
    }

}
