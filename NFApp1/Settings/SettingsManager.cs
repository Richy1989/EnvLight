using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using nanoFramework.Json;
using NFApp1.Manager;

namespace NFApp1.Settings
{
    public class SettingsManager
    {
        public string uniqueID;
        private const string FilePath = "I:\\Seetings.Json";
        private ManualResetEvent mreSettings = new ManualResetEvent(true);

        public Settings GlobalSettings { get; private set; }
        public void LoadSettings(bool resetSettings = false)
        {
            mreSettings.WaitOne();

            //Delete Settings File
            if (resetSettings)
            {
                DeleteSettingsFile();
            }

            //If not exist -> Create new Setting File
            if (!File.Exists(FilePath))
            {
                Debug.WriteLine("+++++ Create new Settings File +++++");
                Settings newSettings = new();
                uniqueID = EnvLightManager.GetUniqueID();
                newSettings.MqttSettings.MqttClientID = string.Format("EnvLight_{0}", uniqueID);

                CreateSettingFile(newSettings);
            }

            //Read settings from settings file
            Debug.WriteLine("+++++ Read settings from file +++++");
            FileStream fs2 = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite);
            byte[] fileContent = new byte[fs2.Length];
            fs2.Read(fileContent, 0, (int)fs2.Length);

            var settingsText = Encoding.UTF8.GetString(fileContent, 0, (int)fs2.Length);

            Debug.WriteLine("+++++ Settings Text: +++++");
            Debug.WriteLine(settingsText);

            this.GlobalSettings = (Settings)JsonConvert.DeserializeObject(settingsText, typeof(Settings));

            mreSettings.Set();
        }

        public void UpdateSettings()
        {
            mreSettings.WaitOne();

            DeleteSettingsFile();

            if (GlobalSettings != null)
                CreateSettingFile(GlobalSettings);

            mreSettings.Set();
        }

        private void CreateSettingFile(Settings settingsFile)
        {
            File.Create(FilePath);
            FileStream fileStream = new(FilePath, FileMode.Open, FileAccess.ReadWrite);
            var newSettingsText = JsonConvert.SerializeObject(settingsFile);
            var settingsBuffer = Encoding.UTF8.GetBytes(newSettingsText);
            fileStream.Write(settingsBuffer, 0, settingsBuffer.Length);
            fileStream.Dispose();
        }

        private void DeleteSettingsFile()
        {
            Debug.WriteLine("+++++ Deleting Settings File +++++");
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}