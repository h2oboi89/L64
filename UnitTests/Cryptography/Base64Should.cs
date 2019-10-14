using Cryptography;
using NUnit.Framework;

namespace UnitTests.Cryptography
{
    [TestFixture]
    public class Base64Should
    {
        [Test]
        public void EncodeShouldReturnEncodedString()
        {
            Assert.AreEqual("SGVsbG8sIFdvcmxkIQ==", Base64.Encode("Hello, World!"));
            Assert.AreEqual("U0dWc2JHOHNJRmR2Y214a0lRPT0=", Base64.Encode("SGVsbG8sIFdvcmxkIQ=="));

            Assert.AreEqual("SGVsbG8sIFdvcmxkISAg", Base64.Encode("Hello, World!  "));
        }

        [Test]
        public void DecodeShouldReturnDecodedString()
        {
            Assert.AreEqual("Hello, World!", Base64.Decode("SGVsbG8sIFdvcmxkIQ=="));
            Assert.AreEqual("SGVsbG8sIFdvcmxkIQ==", Base64.Decode("U0dWc2JHOHNJRmR2Y214a0lRPT0="));
        }
    }
}
