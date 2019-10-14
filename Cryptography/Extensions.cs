using System;
using System.Linq;
using System.Security.Cryptography;

namespace Cryptography
{
    public static class Extensions
    {
        public static string Shuffle(this string input)
        {
            using (var rnd = new RNGCryptoServiceProvider())
            {
                return string.IsNullOrEmpty(input) ? string.Empty : new string(input.ToCharArray().OrderBy(x => rnd.GetNextInt32()).ToArray());
            }
        }

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