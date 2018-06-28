using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameLauncher.Data
{
    /// <summary>
    /// The different download types.
    /// "Standard" -> All LODs except Maximum
    /// "Maximum"  -> All LODs
    /// </summary>
    public enum DownloadType
    {
        Standard,
        Maximum
    }

    /// <summary>
    /// The list of game languages.
    /// </summary>
    public enum GameLanguage
    {
        English
    }

    /// <summary>
    /// The structure of the launcher configuration file.
    /// </summary>
    public class LauncherConfig
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public GameLanguage GameLanguage { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadType DownloadType { get; set; }

        [JsonProperty]
        public string LauncherLanguage { get; set; }

        [JsonProperty]
        public string GameDirectory { get; set; }

        [JsonProperty]
        public bool RememberCredentials { get; set; }

        [JsonProperty]
        public string RememberedEmail { get; set; }
    }
}
