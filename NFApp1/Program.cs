using System.Diagnostics;
using System.Threading;
using NFApp1.Manager;

namespace NFApp1
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            EnvLightManager envLightManager = new EnvLightManager();
            
            //controller.ClosePin(Gpio.IO02);
            Thread.Sleep(Timeout.Infinite);
            
            ////while (!token.IsCancellationRequested)
            ////{
            ////    Thread.Sleep(500);
            ////    GpioPin pin = controller.OpenPin(Gpio.IO02, PinMode.Output);
            ////    if (controller.IsPinOpen(Gpio.IO02))
            ////    {
            ////        pin.Write(PinValue.High);
            ////        Thread.Sleep(500);
            ////        pin.Write(PinValue.Low);
            ////        DHTSendsor.ReadOnce();
            ////    }
            ////    controller.ClosePin(Gpio.IO02);
            ////}
        }
    }
}
