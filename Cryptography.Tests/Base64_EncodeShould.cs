using NUnit.Framework;
using Cryptography;

namespace Tests
{
    public class Base64_EncodeShould
    {
        [Test]
        public void ReturnBase64EncodedStringGivenAsciiString()
        {
            Assert.AreEqual("SGVsbG8sIFdvcmxkIQ==", Cryptography.Base64.Encode("Hello, World!"));
            Assert.AreEqual("U0dWc2JHOHNJRmR2Y214a0lRPT0=", Cryptography.Base64.Encode("SGVsbG8sIFdvcmxkIQ=="));
        }
    }
}