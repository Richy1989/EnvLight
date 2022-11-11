using System;
using System.Text;
using NFApp1.MQTT.Interfaces;

namespace NFApp1.MQTT
{
    public class SensorEnvironmentMessage : IMessageBase
    {
        public string Topic { get; set; } = "Environment";
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
