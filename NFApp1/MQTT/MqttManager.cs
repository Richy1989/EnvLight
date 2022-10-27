using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using NFApp1.Interfaces;
using NFApp1.MQTT;

namespace LuminInside.MQTT
{
    public class MqttManager : IPublishMqtt
    {
        private MqttClient mqtt;
        private string clientId;
        private bool closeConnection = false;
        private string ip;
        private string clientID;
        private string user;
        private string password;
        private CancellationToken token;

        public int SendInterval { get; set; }
        public IDictionary Messages { get; set; }

        public MqttManager(CancellationToken token)
        {
            SendInterval = 1000;
            this.token = token;
            Messages = new Hashtable();
            Messages.Add(typeof(SensorEnvironmentMessage), new SensorEnvironmentMessage());
        }

        public void Connect(string ip, string clientID, string user, string password)
        {
            this.ip = ip;
            this.clientID = clientID;
            this.user = user;
            this.password = password;

            closeConnection = false;

            EstablishConnection();

            mqtt.ConnectionClosed += (s, e) =>
            {
                if (!closeConnection)
                {
                    EstablishConnection();
                }
            };
        }

        private void EstablishConnection()
        {
            mqtt = new MqttClient(ip);
            var ret = mqtt.Connect(clientID, user, password);
            this.clientId = clientID;

            if (ret != MqttReasonCode.Success)
            {
                Debug.WriteLine($"ERROR connecting: {ret}");
                mqtt.Disconnect();
                return;
            }

            Debug.WriteLine($"MQTT connecting successful: {ret}");
        }

        public void Publish(string topic, string message)
        {
            SendMessage(topic, Encoding.UTF8.GetBytes(message));
        }

        public void SendMessage(string topic, byte[] message)
        {
            string to = clientId + "/" + topic;
            mqtt.Publish(to, message);
        }

        public void StartSending()
        {
            while (!token.IsCancellationRequested)
            {
                foreach (var message in Messages.Values)
                {
                    string jsonString = nanoFramework.Json.JsonConvert.SerializeObject(message);
                    Publish(((IMessageBase)message).Topic, jsonString);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
