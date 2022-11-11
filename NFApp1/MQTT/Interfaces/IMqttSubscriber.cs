using System;
using nanoFramework.Json;
using NFApp1.MQTT.Commands;

namespace NFApp1.MQTT.Interfaces
{
    public interface IMqttSubscriber
    {
        string Topic { get; set; }
        void Execute(string content);
    }
}
