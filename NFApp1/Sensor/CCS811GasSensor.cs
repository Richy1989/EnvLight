using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Ccs811;
using nanoFramework.Hardware.Esp32;
using NFApp1.Interfaces;
using NFApp1.MQTT;
using UnitsNet;

namespace NFApp1.Sensor
{
    public class CCS811GasSensor
    {
        private readonly Ccs811Sensor sensor;
        private readonly ITemperatureHumidity temperatureHumidity;
        private CancellationToken token;
        private readonly IPublishMqtt publishMqtt;

        public double eCO2 { get; set; }
        public double eTVOC { get; set; }
        public double Current { get; set; }
        public double ADC { get; set; }

        public CCS811GasSensor(byte pinSDA, byte pinCLK, IPublishMqtt publishMqtt, ITemperatureHumidity temperatureHumidity, CancellationToken token)
        {
            this.token = token;
            this.temperatureHumidity = temperatureHumidity;
            this.publishMqtt = publishMqtt;

            Configuration.SetPinFunction(pinSDA, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(pinCLK, DeviceFunction.I2C1_CLOCK);

            var i2cSettings = new I2cConnectionSettings(1, Ccs811Sensor.I2cFirstAddress, I2cBusSpeed.StandardMode);
            var i2cDevice = I2cDevice.Create(i2cSettings);

            i2cDevice.WriteByte((byte)0x20);
            var result = i2cDevice.ReadByte();
            
            sensor = new Ccs811Sensor(i2cDevice);

            this.token = token;

            sensor.OperationMode = OperationMode.ConstantPower1Second;
            Thread adjustThread = new(new ThreadStart(AdjustTemperatureHumidity));
            adjustThread.Start();
        }

        public void StartMeasuring()
        {

            while (!token.IsCancellationRequested)
            {
                while (!sensor.IsDataReady && !token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }

                var success = sensor.TryReadGasData(out VolumeConcentration eCO2, out VolumeConcentration eTVOC, out ElectricCurrent curr, out int adc);

                if (success)
                {
                    Debug.WriteLine($"Success: {success}, eCO2: {eCO2.PartsPerMillion} ppm, eTVOC: {eTVOC.PartsPerBillion} ppb, Current: {curr.Microamperes} µA, ADC: {adc} = {adc * 1.65 / 1023} V.");

                    this.eCO2 = eCO2.PartsPerMillion;
                    this.eTVOC = eTVOC.PartsPerBillion;
                    this.Current = curr.Microamperes;
                    this.ADC = adc * 1.65 / 1023;

                    if (publishMqtt != null)
                    {
                        ((SensorAirQualityMessage)publishMqtt.Messages[typeof(SensorAirQualityMessage)]).TotalVolatileCompound = this.eCO2;
                        ((SensorAirQualityMessage)publishMqtt.Messages[typeof(SensorAirQualityMessage)]).TotalVolatileOrganicCompound = this.eTVOC;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void AdjustTemperatureHumidity()
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromSeconds(60));
                Debug.WriteLine("+++++ Updating Temperature and Humidity in CC811 Sensor +++++");

                if (temperatureHumidity != null)
                    sensor.SetEnvironmentData(Temperature.FromDegreesCelsius(temperatureHumidity.Temperature), RelativeHumidity.FromPercent(temperatureHumidity.Humidity));
                else
                    sensor.SetEnvironmentData(Temperature.FromDegreesCelsius(21.3), RelativeHumidity.FromPercent(42.5));
            }
        }
    }
}
