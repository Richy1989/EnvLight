using System;
using System.Text;

namespace NFApp1.MQTT
{
    public class SensorEnvironmentMessage : IMessageBase
    {
        public string Topic { get; set; } = "Environment";
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
