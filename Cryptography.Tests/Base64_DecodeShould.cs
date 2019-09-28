using NUnit.Framework;
using Cryptography;

namespace Tests
{
    public class Base64_DecodeShould
    {
        [Test]
        public void ReturnAsciiStringGivenBase64EncodedString()
        {
            Assert.AreEqual("Hello, World!", Base64.Decode("SGVsbG8sIFdvcmxkIQ=="));
            Assert.AreEqual("SGVsbG8sIFdvcmxkIQ==", Base64.Decode("U0dWc2JHOHNJRmR2Y214a0lRPT0="));
        }
    }
}