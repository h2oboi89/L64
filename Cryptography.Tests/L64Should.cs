using NUnit.Framework;
using Cryptography;

namespace Tests
{
    public class L64Should
    {
        [Test]
        public void EncryptAndDecrypt()
        {
            var plainText = "Hello, World!";
            var key = L64.GenerateKey();

            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, key);

            Assert.AreNotEqual(plainText, cipherText);
            Assert.AreNotSame(plainText, cipherText);

            Assert.AreEqual(plainText, decrypted);
            Assert.AreNotSame(plainText, decrypted);

            Assert.AreNotEqual(decrypted, cipherText);
            Assert.AreNotSame(decrypted, cipherText);
        }
    }
}