using System;
using System.Collections.Generic;
using GameLauncher.Data;

namespace GameLauncher.Services
{
    /// <summary>
    /// A service to manage launcher localization.
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Load all of the available language packs.
        /// </summary>
        void LoadPacks();

        /// <summary>
        /// Load the language pack with the given key.
        /// </summary>
        /// <param name="key">The language pack key.</param>
        /// <exception cref="ArgumentException">Thrown when the language pack could not be found.</exception>
        void LoadPack(string key);

        /// <summary>
        /// Get the list of available language pack keys.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of the available language keys.</returns>
        List<string> GetAllLanguages();

        /// <summary>
        /// Get the language pack with the given key.
        /// </summary>
        /// <param name="key">The language pack key.</param>
        /// <returns>The <see cref="LanguagePack"/> instance with the given key.</returns>
        /// <exception cref="ArgumentException">Thrown when the language pack could not be found.</exception> 
        LanguagePack GetLanguagePack(string key);
    }
}
