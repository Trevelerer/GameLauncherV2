using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameLauncher.Data
{
    /// <summary>
    /// The structure of a language pack file.
    /// </summary>
    public class LanguagePack
    {
        /// <summary>
        /// The name of the language pack.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The author of the language pack.
        /// </summary>
        [JsonProperty]
        public string Author { get; set; }

        /// <summary>
        /// The phrase map.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> Phrases { get; set; }

        /// <summary>
        /// Get a phrase from the pack.
        /// </summary>
        /// <param name="key">The phrase key.</param>
        /// <param name="formatParams">The formatting parameters.</param>
        /// <returns>The phrase value, or <c>Unlocalized String</c> if the phrase key is unknown.</returns>
        public string GetPhrase(string key, params object[] formatParams)
        {
            return !Phrases.ContainsKey(key) 
                ? "Unlocalized String" 
                : string.Format(Phrases[key], formatParams);
        }
    }
}
