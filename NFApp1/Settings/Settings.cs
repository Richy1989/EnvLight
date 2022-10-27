using nanoFramework.Hardware.Esp32;

namespace NFApp1.Settings
{
    public class Settings
    {
        public bool UseLedLight { get; set; } = true;
        public byte SPIMOSI { get; set; } = Gpio.IO12;
        public byte SPICLK { get; set; } = Gpio.IO14;

        public bool UseLedSwitches { get; set; } = true;
        public byte LightOnOffLeftSwitch { get; set; } = Gpio.IO32;
        public byte LightOnOffRightSwitch { get; set; } = Gpio.IO33;

        public bool UseDHT22 { get; set; } = true;
        public byte DHT22Gpio1 { get; set; } = Gpio.IO25;
        public byte DHT22Gpio2 { get; set; } = Gpio.IO26;

        public bool UseCCS811 { get; set; } = true;
        public byte I2CSDA { get; set; } = Gpio.IO21;
        public byte I2CSCL { get; set; } = Gpio.IO22;

        public WifiSettings WifiSettings { get; set; } = new WifiSettings();
        public MqttSettings MqttSettings { get; set; } = new MqttSettings();
    }
}
