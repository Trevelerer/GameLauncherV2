using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Utils
{
    /// <summary>
    /// Hashing utilities.
    /// </summary>
    public class Hasher
    {
        /// <summary>
        /// Generate a SHA-1 hash of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The generated hash.</returns>
        public static string HashSHA1(string input)
        {
            var algorithm = SHA1.Create();
            var builder = new StringBuilder();

            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)))
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }
    }
}
