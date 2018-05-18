using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using GameLauncher.Data;

namespace GameLauncher.Classes
{
    /// <summary>
    /// Manages the launcher configuration.
    /// </summary>
    public class Configuration
    {
        private const string ConfigFileName = "config.json";

        private static readonly Lazy<Configuration> Lazy = new Lazy<Configuration>(() => new Configuration());

        private Configuration()
        {
        }

        /// <summary>
        /// Load the launcher configuration from its file.
        /// </summary>
        public void Load()
        {
            if (!Exists)
            {
                throw new FileNotFoundException("Cannot load config. It does not exist.");
            }

            Config = JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText(ConfigFileName));

            Console.WriteLine("Loaded config");
        }

        /// <summary>
        /// Create a new configuration file.
        /// </summary>
        /// <param name="gameDirectory">The game directory to set in the config.</param>
        public void Create(string gameDirectory)
        {
            Config = new LauncherConfig
            {
                GameDirectory = gameDirectory,
                DownloadType = DownloadType.Standard,
                GameLanguage = GameLanguage.English,
                LauncherLanguage = "English"
            };
            Save();
        }
        
        /// <summary>
        /// Save the configuration.
        /// </summary>
        public void Save()
        {
            if (Config == null)
            {
                throw new NullReferenceException(nameof(Config));
            }

            File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(Config));
        }

        /// <summary>
        /// Returns whether the config file exists.
        /// </summary>
        public bool Exists
        {
            get {
                return File.Exists(ConfigFileName); 
            }
        }

        public LauncherConfig Config { get; set; }

        public static Configuration Instance => Lazy.Value;
    }
}
