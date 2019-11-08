using System;
using System.Linq;
using System.Security.Cryptography;

namespace Cryptography
{
    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Shuffles the characters in a string.
        /// </summary>
        /// <param name="input">String to shuffle.</param>
        /// <returns>String with same characters but in a different order.</returns>
        public static string Shuffle(this string input)
        {
            using (var rnd = new RNGCryptoServiceProvider())
            {
                return string.IsNullOrEmpty(input) ? string.Empty : new string(input.ToCharArray().OrderBy(x => rnd.GetNextInt32()).ToArray());
            }
        }

        /// <summary>
        /// Gets next random int.
        /// </summary>
        /// <param name="rnd">Random Number Generator.</param>
        /// <returns>Next random int.</returns>
        public static int GetNextInt32(this RNGCryptoServiceProvider rnd)
        {
            if (rnd == null)
            {
                throw new ArgumentNullException(nameof(rnd));
            }

            var randomInt = new byte[4];
            rnd.GetBytes(randomInt);

            return BitConverter.ToInt32(randomInt, 0);
        }
    }
}