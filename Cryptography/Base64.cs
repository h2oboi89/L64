namespace Cryptography
{
    /// <summary>
    /// Base64 Utility methods
    /// </summary>
    public static class Base64
    {
        /// <summary>
        /// Uses Base64 to encode the input.
        /// </summary>
        /// <param name="input">Text to encode.</param>
        /// <returns>Base64 encoded input.</returns>
        public static string Encode(string input)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);

            return System.Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Uses Base64 to decode the input.
        /// </summary>
        /// <param name="input">Text to decode.</param>
        /// <returns>Base64 decoded input.</returns>
        public static string Decode(string input)
        {
            var bytes = System.Convert.FromBase64String(input);

            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}