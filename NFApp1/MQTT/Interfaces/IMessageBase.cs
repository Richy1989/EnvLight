using System;
using System.Text;

namespace NFApp1.MQTT.Interfaces
{
    internal interface IMessageBase
    {
        string Topic { get; set; }
    }
}
