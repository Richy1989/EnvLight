using NFApp1.MQTT.Interfaces;

namespace NFApp1.MQTT
{
    public class SensorAirQualityMessage : IMessageBase
    {
        public string Topic { get; set; } = "AirQuality";
        public double TotalVolatileCompound { get; set; }
        public string TotalVolatileCompound_Unit { get; set; } = "ppm";
        public double TotalVolatileOrganicCompound { get; set; }
        public string TotalVolatileOrganicCompound_Unit { get; set; } = "ppb";
    }
}
