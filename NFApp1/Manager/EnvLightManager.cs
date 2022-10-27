using LuminInside.MQTT;
using LuminInside.Sensor;
using LuminInside.WiFi;
using nanoFramework.Hardware.Esp32;
using NFApp1.Light;
using NFApp1.Settings;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace NFApp1.Manager
{
    public class EnvLightManager
    {
        public GpioService.GpioService gpioService;
        public SettingsManager SettingsManager { get; set; }
        public Settings.Settings GlobalSettings { get; set; }
        public bool IsLightOn { get; set; }
        public LedManager LedManager { get; set; }
        public string AsseblyName { get; }

        private readonly GpioController controller;

        public EnvLightManager()
        {
            this.AsseblyName = "EnvLight";
            this.SettingsManager = new SettingsManager();
            this.SettingsManager.LoadSettings(true);
            this.GlobalSettings = this.SettingsManager.GlobalSettings;

            //Load the default values only valid for the build environment. Do not make these values Public
#if DEBUG
            Debug.WriteLine("+++++ Write Build Variables to Settings: +++++");
            SettingsManager.GlobalSettings.WifiSettings.ConnectToWifi = true;
            SettingsManager.GlobalSettings.WifiSettings.Ssid = NotPushable.NotPushable.WifiSsid;
            SettingsManager.GlobalSettings.WifiSettings.Password = NotPushable.NotPushable.WifiPassword;
            SettingsManager.GlobalSettings.MqttSettings.ConnectToMqtt = true;
            SettingsManager.GlobalSettings.MqttSettings.MqttUserName = NotPushable.NotPushable.MQTTUserName;
            SettingsManager.GlobalSettings.MqttSettings.MqttPassword = NotPushable.NotPushable.MQTTPassword;
            SettingsManager.GlobalSettings.MqttSettings.MqttHostName = NotPushable.NotPushable.MQTTHostName;
#endif

            CancellationTokenSource source = new();
            CancellationToken token = source.Token;

            controller = new GpioController();
            ////LedManager = new(new LedAPA102Controller(50));

            this.gpioService = new GpioService.GpioService(controller, this, SettingsManager.GlobalSettings, token);
            gpioService.Execute();
            //Thread touchThread = new(new ThreadStart(gpioService.Execute));
            //touchThread.Start();

            WiFiManager wifi = new(token);
            wifi.Connect(GlobalSettings.WifiSettings.Ssid, GlobalSettings.WifiSettings.Password);

            Thread wifiThread = new(new ThreadStart(wifi.KeepConnected));
            wifiThread.Start();

            MqttManager mqttManager = new(token);
            mqttManager.Connect(
                GlobalSettings.MqttSettings.MqttHostName,
                string.Format("{0}/{1}", AsseblyName, GlobalSettings.MqttSettings.MqttClientID),
                GlobalSettings.MqttSettings.MqttUserName, 
                GlobalSettings.MqttSettings.MqttPassword);
            
            Thread mqttThread = new(new ThreadStart(mqttManager.StartSending));
            mqttThread.Start();

            var DHTSendsor = new DHT22Sensor(Gpio.IO12, Gpio.IO14, controller, mqttManager, token);
            Thread sensorThread = new(new ThreadStart(DHTSendsor.StartRead));
            sensorThread.Start();

            GpioPin pin = controller.OpenPin(Gpio.IO02, PinMode.Output);
            pin.Write(PinValue.High);
        }

        //Creae aUnique ID based on the MAC address of the controller
        public static string GetUniqueID()
        {
            var ni = NetworkInterface.GetAllNetworkInterfaces();
            if (ni.Length > 0)
            {
                var physicalAddress = ni[0].PhysicalAddress;
                var physicalAddressString = string.Format("{0:X}:{1:X}:{2:X}:{3:X}:{4:X}:{5:X}", physicalAddress[0], physicalAddress[1], physicalAddress[2], physicalAddress[3], physicalAddress[4], physicalAddress[5]);
                Debug.WriteLine(string.Format("+++ Returning MAC: {0} +++", physicalAddressString));
                return physicalAddressString;
            }
            else
            {
                var uniqueID = Guid.NewGuid().ToString();
                Debug.WriteLine(string.Format("+++ Returning GUID: {0} +++", uniqueID));
                return uniqueID;
            }
        }
    }
}
