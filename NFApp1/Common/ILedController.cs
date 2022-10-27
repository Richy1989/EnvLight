using System;
using System.Threading;

namespace LuminInside.Common
{
    /// <summary>
    /// Defines the interface for an LED Controller. 
    /// </summary>
    /// <seealso cref="IDisposable" />
    public interface ILedController : IDisposable
    {
        /// <summary>Gets or sets the dim ratio.</summary>
        /// <value>The dim ratio.</value>
        int Brightness { get; set; }

        /// <summary>
        /// Gets or sets the actual screen.
        /// </summary>
        /// <value>
        /// The actual screen.
        /// </value>
        LedPixel[] ActualScreen { get; set; }

        /// <summary>Gets or sets the clock speed in Hz.</summary>
        /// <value>The clock speed oin Hz.</value>
        int ClockSpeedHz { get; set; }

        /// <summary>Gets or sets the pixel delta.</summary>
        /// <value>The pixel delta.</value>
        int PixelOffset { get; set; }

        /// <summary>Sends the pixels.</summary>
        /// <param name="pixels">The pixels.</param>
        void SendPixels(LedPixel[] pixels);

        /// <summary>Gets the height of the get.</summary>
        /// <value>The height of the get.</value>
        int LedCount { get; set; }

        /// <summary>Gets or sets the CancellationToken.</summary>
        /// <value>The token.</value>
        CancellationToken Token { get; set; }
    }
}
