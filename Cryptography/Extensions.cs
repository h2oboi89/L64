using System;

namespace Cryptography
{
    public static class Extensions
    {
        public static string Shuffle(this string input)
        {
            var random = new Random();
            var array = input.ToCharArray();

            for (var i = array.Length - 1; i > 1; i--)
            {
                var j = random.Next(0, i);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            return new string(array);
        }
    }
}