using System.Diagnostics;
using nanoFramework.Json;
using NFApp1.MQTT.Interfaces;

namespace NFApp1.MQTT.Commands
{
    public class MqttCommandLightOnOff : IMqttSubscriber
    {
        public string Topic { get; set; } = MqttCommands.OnOffCommand;

        public void Execute(string content)
        {
            var parameters = (CommandOnOffParameter)JsonConvert.DeserializeObject(content, typeof(CommandOnOffParameter));
            Debug.WriteLine($"Parameter: {parameters.On}");
        }
    }

    public class CommandOnOffParameter
    {
        public bool On { get; set; }
    }
}
