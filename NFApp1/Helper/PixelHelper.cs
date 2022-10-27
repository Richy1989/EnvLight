namespace LuminInside.Helper
{
    public static class PixelHelper
    {
        /// <summary>Calculates the real pixel value considering the pixel delta.</summary>
        /// <param name="pixelIndex">Index of the pixel when considered no offset.</param>
        /// <returns>Index of the pixel with all offsets considered.</returns>
        public static int CalculateRealPixel(int pixelIndex, int pixelCount, int pixelelta)
        {
            int newIndex = pixelIndex + pixelelta;

            while (newIndex >= pixelCount)
                newIndex -= pixelCount;

            while (newIndex < 0)
                newIndex += pixelCount;

            return newIndex;
        }
    }
}
