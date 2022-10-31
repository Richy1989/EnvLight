// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.DHTxx.Esp32;
using NFApp1.Interfaces;
using NFApp1.MQTT;
using NFApp1.Sensor;

namespace LuminInside.Sensor
{
    public class DHT22Sensor : ITemperatureHumidity
    {
        public int ReadInterval { get; set; } = 1000;
        public double Temperature { get; set; }
        public double Humidity { get; set; }

        private readonly IPublishMqtt publisher;
        private CancellationToken token;
        private readonly DhtBase dht22;

        public DHT22Sensor(int pinEcho, int pinTrigger, GpioController controller, CancellationToken token)
        {
            this.token = token;
            dht22 = new Dht22(pinEcho, pinTrigger, PinNumberingScheme.Logical, controller);
        }

        public DHT22Sensor(int pinEcho, int pinTrigger, GpioController controller, IPublishMqtt publisher, CancellationToken token)
        {
            this.token = token;
            this.publisher = publisher;
            dht22 = new Dht22(pinEcho, pinTrigger, PinNumberingScheme.Logical, controller);
        }
        public void ReadOnce()
        {
            try
            {
                var temp = dht22.Temperature;
                var hum = dht22.Humidity;

                // You can only display temperature and humidity if the read is successful otherwise, this will raise an exception as
                // both temperature and humidity are NAN
                if (dht22.IsLastReadSuccessful)
                {
                    Debug.WriteLine($"Temperature: {temp.DegreesCelsius}\u00B0C, Relative humidity: {hum.Percent}%");
                    Temperature = temp.DegreesCelsius;
                    Humidity = hum.Percent;

                    if (publisher != null)
                    {
                        ((SensorEnvironmentMessage)publisher.Messages[typeof(SensorEnvironmentMessage)]).Humidity = Humidity;
                        ((SensorEnvironmentMessage)publisher.Messages[typeof(SensorEnvironmentMessage)]).Temperature = Temperature;
                    }
                }
                else
                {
                    Debug.WriteLine("Error reading DHT sensor");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error reading DHT sensor. Message:" + ex.Message);
            }
        }

        public void StartRead()
        {
            while (!token.IsCancellationRequested)
            {
                ReadOnce();
                //Thread.Sleep(ReadInterval);
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
    }
}