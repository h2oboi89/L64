using NUnit.Framework;
using Cryptography;
using System;

namespace Tests
{
    public class L64Should
    {
        [Test]
        public void EncryptAndDecryptSimpleString()
        {
            var plainText = "Hello, World!";
            var key = L64.GenerateKey();

            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, key).Trim();

            Assert.That(plainText, Is.Not.EqualTo(cipherText));
            Assert.That(plainText, Is.Not.SameAs(cipherText));

            Assert.That(plainText, Is.EqualTo(decrypted));
            Assert.That(plainText, Is.Not.SameAs(decrypted));

            Assert.That(decrypted, Is.Not.EqualTo(cipherText));
            Assert.That(decrypted, Is.Not.SameAs(cipherText));
        }

        [Test]
        public void FailToDecryptWithWrongKey()
        {
            var plainText = "Foo Bar";
            var key = L64.GenerateKey();
            var invalidKey = L64.GenerateKey();

            Assert.That(key, Is.Not.EqualTo(invalidKey));
            
            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, invalidKey).Trim();

            Assert.That(plainText, Is.Not.EqualTo(decrypted));
        }

        [Test]
        public void EncryptAndDecryptBlankString()
        {
            var plainText = "   ";
            var key = L64.GenerateKey();

            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, key);

            Assert.That(plainText, Is.Not.EqualTo(cipherText));

            Assert.That(plainText, Is.EqualTo(decrypted));
        }

        [Test]
        public void EncryptAndDecryptEmptyString()
        {
            var plainText = string.Empty;
            var key = L64.GenerateKey();

            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, key);

            Assert.That(plainText, Is.EqualTo(cipherText));

            Assert.That(plainText, Is.EqualTo(decrypted));
        }

        [Test]
        public void RejectInvalidKeys()
        {
            var invalidKeys = new string[] { string.Empty, " ", "abcde" };

            foreach (var invalidKey in invalidKeys)
            {
                var ex = Assert.Throws<ArgumentException>(() => L64.Encrypt(string.Empty, invalidKey));

                Assert.AreEqual(ex.Message, $"Invalid key: '{invalidKey}'. Expected a shuffled version of '+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'");
            }
        }
    }
}