using NFApp1.Settings;

namespace HeliosClockAPIStandard.GpioService
{
    //Mapping GPIO pin to board pin
    public class GpioInputPin
    {
        /// <summary>Initializes a new instance of the <see cref="GpioInputPin"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public GpioInputPin(Settings configuration)
        {
            LeftSide = configuration.LightOnOffLeftSwitch;
            RightSide = configuration.LightOnOffRightSwitch;
        }
        
        public byte LeftSide { get; set; }
        public byte RightSide { get; set; }
    }
}