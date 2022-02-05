using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Aijkl.VR.MicController
{
    internal class AppSettings
    {
        [JsonIgnore] 
        private const string FilePath = "./Resources/appsettings.json";

        [JsonProperty("applicationID")]
        internal string ApplicationID { set; get; }

        [JsonProperty("applicationManifestPath")]
        internal string ApplicationManifestPath { set; get; }

        [JsonProperty("targetMicId")]
        internal string TargetMicId { set; get; }

        [JsonProperty("buttonId")]
        internal int ButtonId { set; get; } 

        [JsonProperty("soundFilePath")]
        internal string SoundFilePath { set; get; }

        [JsonProperty("languageDataSet")]
        public LanguageDataSet LanguageDataSet { set; get; }

        internal static AppSettings LoadFromFile()
        {
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Path.GetFullPath(FilePath)));
        }

        internal void SaveToFile()
        {
            File.WriteAllText(Path.GetFullPath(FilePath),JsonConvert.SerializeObject(this,Formatting.Indented));
        }
    }

    internal class LanguageDataSet
    {
        internal const string CONFIGURE_FILE_ERROR = "Configuration file error";
        internal const string ERROR = "An error has occurred";
        internal string GetValue(string memberName)
        {
            Dictionary<string, string> keyValuePairs = (Dictionary<string, string>)GetType().GetProperty(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(this);
            if (keyValuePairs.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, out string value))
            {
                return value;
            }
            else
            {
                value = keyValuePairs.ToList().FirstOrDefault().Value;
                value = string.IsNullOrEmpty(value) ? string.Empty : value;
                return value;
            }
        }

        [JsonProperty("SelectMic.Message")]
        public Dictionary<string, string> SelectMicMessage { set; get; }

        [JsonProperty("SelectMic.NotFoundMic")]
        public Dictionary<string, string> SelectMicNotFoundMic { set; get; }

        [JsonProperty("SelectMic.Success")]
        public Dictionary<string, string> SelectMicSuccess { set; get; }

        [JsonProperty("General.Configure")]
        public Dictionary<string, string> GeneralConfigure { set; get; }

        [JsonProperty("General.Exit")]
        public Dictionary<string, string> GeneralExit { set; get; }

        [JsonProperty("General.MutexError")]
        public Dictionary<string, string> GeneralMutexError { set; get; }

        [JsonProperty("Config.Error")]
        public Dictionary<string, string> ConfigError { set; get; }

        [JsonProperty("OpenVR.Initializing")]
        public Dictionary<string, string> OpenVRInitializing { set; get; }

        [JsonProperty("OpenVR.InitError")]
        public Dictionary<string, string> OpenVRInitError { set; get; }

        [JsonProperty("StreamVR.AddManifest.Success")]
        public Dictionary<string, string> StreamVRAddManifestSuccess { set; get; }

        [JsonProperty("StreamVR.AddManifest.Failure")]
        public Dictionary<string, string> StreamVRAddManifestFailure { set; get; }

        [JsonProperty("StreamVR.RemoveManifest.Success")]
        public Dictionary<string, string> StreamVRRemoveManifestSuccess { set; get; }

        [JsonProperty("StreamVR.RemoveManifest.Failure")]
        public Dictionary<string, string> StreamVRRemoveManifestFailure { set; get; }
    }
}
