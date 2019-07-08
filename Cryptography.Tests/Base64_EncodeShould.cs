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
        }
    }
}