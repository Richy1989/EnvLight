using nanoFramework.Json;
using NFApp1.Manager;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NFApp1.Settings
{
    public class SettingsManager
    {
        public string uniqueID;
        public Settings GlobalSettings  { get; private set; }
        public void LoadSettings(bool resetSettings = false)
        {
            var filePath = "I:\\Seetings.Json";

            //Delete Settinga FIle
            if (resetSettings)
            {
                Debug.WriteLine("+++++ Deleting Settings File +++++");
                if(File.Exists(filePath))
                    File.Delete(filePath);
            }

            //If not exist -> Create new Setting File
            if (!File.Exists(filePath))
            {
                Debug.WriteLine("+++++ Create new Settings File +++++");
                Settings newSettings = new();
                uniqueID = EnvLightManager.GetUniqueID();
                newSettings.MqttSettings.MqttClientID = string.Format("EnvLight_{0}", uniqueID);

                File.Create(filePath);
                FileStream fileStream = new(filePath, FileMode.Open, FileAccess.ReadWrite);
                var newSettingsText = JsonConvert.SerializeObject(newSettings);
                var settingsBuffer = Encoding.UTF8.GetBytes(newSettingsText);
                fileStream.Write(settingsBuffer, 0, settingsBuffer.Length);
                fileStream.Dispose();
            }

            //Read settings from settings file
            Debug.WriteLine("+++++ Read settings from file +++++");
            FileStream fs2 = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            byte[] fileContent = new byte[fs2.Length];
            fs2.Read(fileContent, 0, (int)fs2.Length);

            var settingsText = Encoding.UTF8.GetString(fileContent, 0, (int)fs2.Length);

            Debug.WriteLine("+++++ Settings Text: +++++");
            Debug.WriteLine(settingsText);

            this.GlobalSettings = (Settings)JsonConvert.DeserializeObject(settingsText, typeof(Settings));
        }
    }
}
