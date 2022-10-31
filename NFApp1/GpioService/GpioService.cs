using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using HeliosClockAPIStandard.GpioService;
using NFApp1.Enumerations;
using NFApp1.Manager;

namespace NFApp1.GpioService
{
    public class GpioService
    {
        private readonly EnvLightManager manager;
        private readonly GpioController gpioController;

        private readonly Stopwatch stopwatchLeft;
        private readonly Stopwatch stopwatchRight;

        private int touchCount = 0;

        private bool isOn = false;
        private bool isLeftOn = false;
        private bool isRightOn = false;
        private bool firstLongToucgOccurred = false;

        private PinValue pinLeftOld = PinValue.Low;
        private PinValue pinRightOld = PinValue.Low;

        private readonly Color onColor = DefaultColors.WarmWhite;

        public LedSide side;
        private GpioInputPin gpioInputPin;
        private CancellationToken stoppingToken;

        /// <summary>Initializes a new instance of the <see cref="GPIOService"/> class.</summary>
        /// <param name="manager">The manager.</param>
        public GpioService(GpioController gpioController, EnvLightManager manager, Settings.Settings configuration, CancellationToken stoppingToken)
        {
            
            this.stoppingToken = stoppingToken;
            this.manager = manager;
            this.gpioController = gpioController;
            stopwatchLeft = new Stopwatch();
            stopwatchRight = new Stopwatch();

            gpioInputPin = new GpioInputPin(configuration);
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        public void Execute()
        {
            gpioController.OpenPin(gpioInputPin.LeftSide, PinMode.Input);
            gpioController.OpenPin(gpioInputPin.RightSide, PinMode.Input);

            gpioController.RegisterCallbackForPinValueChangedEvent(gpioInputPin.LeftSide, PinEventTypes.Falling | PinEventTypes.Rising, (s, e) =>
            {
                PinValue pinV = e.ChangeType == PinEventTypes.Falling ? PinValue.Low : PinValue.High;
                ExecuteTouchWatcher(LedSide.Left, pinV, side == LedSide.Left ? stopwatchLeft : stopwatchRight);
            });

            gpioController.RegisterCallbackForPinValueChangedEvent(gpioInputPin.RightSide, PinEventTypes.Falling | PinEventTypes.Rising, (s, e) =>
            {
                PinValue pinV = e.ChangeType == PinEventTypes.Falling ? PinValue.Low : PinValue.High;
                ExecuteTouchWatcher(LedSide.Right, pinV, side == LedSide.Left ? stopwatchLeft : stopwatchRight);
            });

            Debug.WriteLine(string.Format("Started GPIO Watch ..."));

            ////try
            ////{
            ////    gpioController.OpenPin(gpioInputPin.LeftSide, PinMode.Input);
            ////    gpioController.OpenPin(gpioInputPin.RightSide, PinMode.Input);

            ////    var side = LedSide.Left;

            ////    while (!stoppingToken.IsCancellationRequested)
            ////    {
            ////        isOn = manager.IsLightOn;

            ////        var input = gpioController.Read(side == LedSide.Left ? gpioInputPin.LeftSide : gpioInputPin.RightSide);
            ////        ExecuteTouchWatcher(side, input, side == LedSide.Left ? stopwatchLeft : stopwatchRight);
            ////        side = side == LedSide.Left ? LedSide.Right : LedSide.Left;
            ////        Thread.Sleep(5);
            ////    }

            ////    gpioController.ClosePin(gpioInputPin.LeftSide);
            ////    gpioController.ClosePin(gpioInputPin.RightSide);
            ////}
            ////catch (Exception ex)
            ////{
            ////    Debug.WriteLine(string.Format("Error Read Pin GPIO Service. Message: {0} ...", ex.Message));
            ////}

            ////Debug.WriteLine(string.Format("Stopped GPIO Watch ..."));
        }

        /// <summary>Executes the touch watcher. Checks if short or long press. On or Off.</summary>
        /// <param name="side">The side.</param>
        /// <param name="stopwatch">The stopwatch.</param>
        private void ExecuteTouchWatcher(LedSide side, PinValue input, Stopwatch stopwatch)
        {
            long elapsed = 0;

            //Check for flip
            CheckTouchFlipSetOld(side, input);

            if (input == PinValue.High)
            {
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                    touchCount = 1;
                    firstLongToucgOccurred = false;
                }
            }

            elapsed = stopwatch.ElapsedMilliseconds;

            //If touch is released
            if (input == PinValue.Low && stopwatch.IsRunning)
            {
                stopwatch.Stop();
                elapsed = stopwatch.ElapsedMilliseconds;
                Debug.WriteLine(string.Format("Touch duration: {0} ms ...", elapsed));
                stopwatch.Reset();
            }

            //If touch is pressed continuously
            if (stopwatch.IsRunning)
            {
                //Random Color Full
                if (elapsed - touchCount * TouchDefaultValues.MinLongPressDuration >= TouchDefaultValues.MinLongPressDuration)
                {
                    Debug.WriteLine(string.Format("Touch random color ..."));

                    touchCount++;

                    //ToDo: Implement in Manager
                    manager.LedManager.SetRandomColor();
                    //manager.NotifyControllers();
                    return;
                }

                //First long press, only white / black in full mode
                if (elapsed >= TouchDefaultValues.MinLongPressDuration && !firstLongToucgOccurred)
                {
                    Debug.WriteLine(string.Format("First long press. Mode: {0} ...", isOn ? PowerOnOff.Off : PowerOnOff.On));

                    side = LedSide.Full;

                    //ToDo: Implement in Manager
                    manager.LedManager.LightOnOff(isOn ? PowerOnOff.Off : PowerOnOff.On, side, onColor);
                    //manager.NotifyControllers();

                    //Flip between on off
                    isOn = !isOn;

                    if (isOn)
                    {
                        isLeftOn = true;
                        isRightOn = true;
                    }
                    else
                    {
                        isLeftOn = false;
                        isRightOn = false;
                    }

                    firstLongToucgOccurred = true;
                }
                return;
            }

            if (elapsed >= TouchDefaultValues.MinShortPressDuration && elapsed < TouchDefaultValues.MinLongPressDuration)
            {
                if (side == LedSide.Left)
                {
                    //ToDo: Implement in Manager
                    manager.LedManager.LightOnOff(isLeftOn ? PowerOnOff.Off : PowerOnOff.On, side, onColor);
                    //manager.NotifyControllers();

                    isLeftOn = !isLeftOn;
                }
                if (side == LedSide.Right)
                {
                    //ToDo: Implement in Manager
                    manager.LedManager.LightOnOff(isRightOn ? PowerOnOff.Off : PowerOnOff.On, side, onColor);
                    //manager.NotifyControllers();

                    isRightOn = !isRightOn;
                }
            }
        }

        /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public void StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (gpioController.IsPinOpen(gpioInputPin.LeftSide))
                    gpioController.ClosePin(gpioInputPin.LeftSide);

                if (gpioController.IsPinOpen(gpioInputPin.RightSide))
                    gpioController.ClosePin(gpioInputPin.RightSide);

                gpioController.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error in GPIO Service. Message: {0}", ex.Message));
            }
        }

        /// <summary>Checks the touch flip.</summary>
        /// <param name="side">The side.</param>
        /// <param name="input">The input.</param>
        private bool CheckTouchFlipSetOld(LedSide side, PinValue input)
        {
            if (side == LedSide.Left)
            {
                if (pinLeftOld != input)
                {
                    Debug.WriteLine(string.Format("Switch {0} toggled. Value: {1} ...", side, input));
                    pinLeftOld = input;
                    return true;
                }
            }
            if (side == LedSide.Right)
            {
                if (pinRightOld != input)
                {
                    Debug.WriteLine(string.Format("Switch {0} toggled. Value: {1} ...", side, input));
                    pinRightOld = input;
                    return true;
                }
            }
            return false;
        }
    }
}
