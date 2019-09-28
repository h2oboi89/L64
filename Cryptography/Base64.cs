namespace Cryptography
{
    public static class Base64
    {
        public static string Encode(string input)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);

            return System.Convert.ToBase64String(bytes);
        }

        public static string Decode(string input)
        {
            var bytes = System.Convert.FromBase64String(input);

            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}