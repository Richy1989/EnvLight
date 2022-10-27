using System;
using System.Text;

namespace NFApp1.MQTT
{
    internal interface IMessageBase
    {
        string Topic { get; set; }
    }
}
