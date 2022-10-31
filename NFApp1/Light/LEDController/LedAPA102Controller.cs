using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using LuminInside.Common;
using LuminInside.Extensions;
using LuminInside.Helper;
using nanoFramework.Hardware.Esp32;

namespace HeliosClockAPIStandard.Controller
{
    public class LedAPA102Controller : ILedController
    {
        // <summary>Line the SPI library uses to signal chip select.</summary>
        /// <remarks>The APA102 doesn't actually support this line, so safe to ignore this.</remarks>
        private const int SpiChipSelectLine = 12;

        /// <summary>Object for communicating with the LED strip.</summary>
        private readonly SpiDevice spiDevice;

        /// <summary>Gets a value representing the count of pixels in the LED strip.</summary>
        public byte[] EndFrame { get; private set; }

        public int ClockSpeedHz { get; set; }

        public int LedCount { get; set; }

        public LedPixel[] ActualScreen { get; set; }
        public int PixelOffset { get; set; }
        public CancellationToken Token { get; set; }
        public int GlobalBrightness { get; set; } = 255;

        /// <summary>The start frame</summary>
        private readonly byte[] startFrame = { 0, 0, 0, 0 };

        /// <summary>The connection settings.</summary>
        private readonly SpiConnectionSettings connectionSettings;
        private bool disposed;

        /// <summary>Constructor.</summary>
        /// <param name="numLeds">Number of LEDs in the strip</param>
        public LedAPA102Controller(int ledCount, byte pinMISO, byte pinCLK) : base()
        {
            Configuration.SetPinFunction(pinMISO, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(pinCLK, DeviceFunction.SPI1_CLOCK);

            LedCount = ledCount;

            int endFrameSize = (int)Math.Ceiling((((double)LedCount) - 1.0) / 16.0);

            // By initializing an int array of that specific length, it gets initialized with ints of default value (0).  :)
            this.EndFrame = new byte[endFrameSize];

            ClockSpeedHz = 20_000_000;

            connectionSettings = new SpiConnectionSettings(1, SpiChipSelectLine)
            {
                ClockFrequency = ClockSpeedHz,
                DataFlow = DataFlow.MsbFirst,
                Mode = SpiMode.Mode0
            };

            spiDevice = GetSpiDevice();
        }

        /// <summary>Sends the pixels.</summary>
        /// <param name="pixels">The pixels.</param>
        public void SendPixels(LedPixel[] pixels)
        {
            int smoothDelta = 1; // ms
            int SmoothTime = 1;

            Color sendColor;

            //var buffer = new byte[(LedCount + 2) * 4];
            var buffer = new byte[(LedCount + 1) * 4 + EndFrame.Length];

            // Original code: _buffer.AsSpan(0, 4).Fill(0x00); // Start Frame
            for (int i = 0; i < 4; i++)
            {
                buffer[i] = 0x00;
            }

            ////// Original code: _buffer.AsSpan((length + 1) * 4, 4).Fill(0xFF); // End Frame
            ////for (int i = (LedCount + 1) * 4; i < ((LedCount + 1) * 4) + 4; i++)
            ////{
            ////    buffer[i] = 0xFF;
            ////}
            ///

            // Original code: End Frame
            for (int i = (LedCount + 1) * 4; i < ((LedCount + 1) * 4) + EndFrame.Length; i++)
            {
                buffer[i] = 0x00;
            }

            for (int smoothIndex = 0; smoothIndex < SmoothTime; smoothIndex += smoothDelta)
            {
                ArrayList spiDataBytes = new ArrayList();
                spiDataBytes.AddRange(startFrame);

                for (int i = 0; i < pixels.Length; i++)
                {
                    int realIndex = PixelHelper.CalculateRealPixel(i, LedCount, PixelOffset);

                    if (pixels[i] == null) pixels[i] = new LedPixel();

                    sendColor = pixels[realIndex].LedColor;

                    SpanByte pixel = buffer;
                    pixel = pixel.Slice((i + 1) * 4);
                    pixel[0] = (byte)((GlobalBrightness >> 3) | 0b11100000); // global brightness (alpha)
                    pixel[1] = sendColor.B; // blue
                    pixel[2] = sendColor.G; // green
                    pixel[3] = sendColor.R; // red
                }

                try
                {
                    spiDevice.Write(buffer);
                    ActualScreen = pixels;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>Gets the SpiDevice handle</summary>
        /// <returns>Task of type SpiDevice, whose result will be the SpiDevice requested if successful</returns>
        private SpiDevice GetSpiDevice()
        {
            Debug.WriteLine("Creating SPI Device ...");
            var device = SpiDevice.Create(connectionSettings);
            Debug.WriteLine("SPI Device Created ...");
            return device;
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                spiDevice?.Dispose();
            }

            disposed = true;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
