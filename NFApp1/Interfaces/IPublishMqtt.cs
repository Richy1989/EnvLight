using System;
using System.Collections;
using System.Text;

namespace NFApp1.Interfaces
{
    public  interface IPublishMqtt
    {
        void Publish(string topic, string message);
        IDictionary Messages { get; set; }
    }
}
