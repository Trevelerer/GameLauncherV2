using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameLauncher.Data;
using Newtonsoft.Json;

namespace GameLauncher.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly Dictionary<string, LanguagePack> _languagePacks;

        /// <summary>
        /// Initialize the language service.
        /// </summary>
        public LanguageService()
        {
            _languagePacks = new Dictionary<string, LanguagePack>();
        }

        public void LoadPacks()
        {
            var packIds = GetAllLanguages();

            foreach (var packId in packIds)
            {
                LoadPack(packId);
            }
        }

        public void LoadPack(string key)
        {
            var path = Path.Combine("Languages", $"{key}.json");

            if (File.Exists(path))
            {
                var contents = File.ReadAllText(path);
                var pack = JsonConvert.DeserializeObject<LanguagePack>(contents);

                _languagePacks[key] = pack;
            }
            else
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var resName = $"{executingAssembly.GetName().Name}.Resources.Languages.{key}.json";

                using (var stream = executingAssembly.GetManifestResourceStream(resName))
                {
                    if (stream == null)
                    {
                        throw new ArgumentException("Unknown language", nameof(key));
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        var contents = reader.ReadToEnd();
                        var pack = JsonConvert.DeserializeObject<LanguagePack>(contents);

                        _languagePacks[key] = pack;
                    }
                }
            }
        }

        public List<string> GetAllLanguages()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var folderName = $"{executingAssembly.GetName().Name}.Resources.Languages";

            // Merges embedded resources with local files

            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".json"))
                .Select(r => r.Substring(folderName.Length + 1))
                .Select(s => s.Replace(".json", ""))
                .ToList()
                .Concat(
                    Directory.Exists("languages") ? Directory.GetFiles("Languages")
                        .Select(s =>
                            s.Substring(s.IndexOf(Path.DirectorySeparatorChar) + 1))
                        .Select(s => s.Replace(".json", "")) : new List<string>()
                )
                .Distinct()
                .ToList();

            //return Directory.GetFiles("Languages")
            //    .Select(s =>
            //        s.Substring(s.IndexOf(Path.DirectorySeparatorChar) + 1))
            //    .Select(s => s.Replace(".json", ""))
            //    .ToList();
        }

        public LanguagePack GetLanguagePack(string key)
        {
            if (!_languagePacks.ContainsKey(key))
            {
                throw new ArgumentException("Unknown language", nameof(key));
            }

            return _languagePacks[key];
        }
    }
}
