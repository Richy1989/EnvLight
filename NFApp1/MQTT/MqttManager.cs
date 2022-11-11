using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using nanoFramework.Json;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using NFApp1.Interfaces;
using NFApp1.MQTT;
using NFApp1.MQTT.Commands;
using NFApp1.MQTT.Interfaces;

namespace LuminInside.MQTT
{
    public class MqttManager : IPublishMqtt
    {
        private MqttClient mqtt;
        private bool closeConnection = false;
        private string ip;
        private string clientID;
        private string user;
        private string password;
        public IDictionary SubscribeTopics;
        private CancellationToken token;


        public int SendInterval { get; set; }
        public IDictionary Messages { get; set; }

        public MqttManager(CancellationToken token)
        {
            SendInterval = 1000;
            this.token = token;
            Messages = new Hashtable();
            Messages.Add(typeof(SensorEnvironmentMessage), new SensorEnvironmentMessage());
            Messages.Add(typeof(SensorAirQualityMessage), new SensorAirQualityMessage());
            SubscribeTopics = new Hashtable();
        }

        public void Connect(string ip, string clientID, string user, string password)
        {
            this.ip = ip;
            this.clientID = clientID;
            this.user = user;
            this.password = password;

            closeConnection = false;
        }

        public void InitializeMQTT()
        {
            EstablishConnection();

            MqttCommandLightOnOff dd = new ();
            AddSubcriber(dd);

            string[] topics = new string[SubscribeTopics.Keys.Count];
            MqttQoSLevel[] level = new MqttQoSLevel[SubscribeTopics.Keys.Count];

            int i = 0;
            foreach (var item in SubscribeTopics.Keys)
            {
                topics[i] = (string)item;
                level[i++] = MqttQoSLevel.ExactlyOnce;
            }

            if (topics.Length > 0)
            {
                mqtt.Subscribe(topics, level);
                mqtt.MqttMsgPublishReceived += Mqtt_MqttMsgPublishReceived;
            }
        }

        private void Mqtt_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.WriteLine($"MQTT Command Received:\nTopic:\n{e.Topic}\nContent:\n{Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)}");

            var subscriber = (IMqttSubscriber)SubscribeTopics[e.Topic];
            subscriber?.Execute(Encoding.UTF8.GetString(e.Message, 0, e.Message.Length));
        }

        public void AddSubcriber(IMqttSubscriber subscriber)
        {
            var topic = $"{clientID}/cmd/{subscriber.Topic}";
            SubscribeTopics.Add(topic, subscriber);
        }

        private void EstablishConnection()
        {
            mqtt = new MqttClient(ip);
            var ret = mqtt.Connect(clientID, user, password);

            if (ret != MqttReasonCode.Success)
            {
                Debug.WriteLine($"ERROR connecting: {ret}");
                mqtt.Disconnect();
                return;
            }

            mqtt.ConnectionClosed += (s, e) =>
            {
                if (!closeConnection)
                {
                    EstablishConnection();
                }
            };

            Debug.WriteLine($"MQTT connecting successful: {ret}");
        }

        public void Publish(string topic, string message)
        {
            SendMessage(topic, Encoding.UTF8.GetBytes(message));
        }

        public void SendMessage(string topic, byte[] message)
        {
            string to = $"{clientID}/{topic}";
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
                Thread.Sleep(SendInterval);
            }
        }
    }
}
