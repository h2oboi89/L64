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
            var decrypted = L64.Decrypt(cipherText, key);

            Assert.AreNotEqual(plainText, cipherText);
            Assert.AreNotSame(plainText, cipherText);

            Assert.AreEqual(plainText + "  ", decrypted);
            Assert.AreNotSame(plainText, decrypted);

            Assert.AreNotEqual(decrypted, cipherText);
            Assert.AreNotSame(decrypted, cipherText);
        }

        [Test]
        public void EncryptAndDecryptBlankString()
        {
            var plainText = "   ";
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

        [Test]
        public void EncryptAndDecryptEmptyString()
        {
            var plainText = String.Empty;
            var key = L64.GenerateKey();

            var cipherText = L64.Encrypt(plainText, key);
            var decrypted = L64.Decrypt(cipherText, key);

            Assert.AreEqual(plainText, cipherText);
            Assert.AreSame(plainText, cipherText);

            Assert.AreEqual(plainText, decrypted);
            Assert.AreSame(plainText, decrypted);

            Assert.AreEqual(decrypted, cipherText);
            Assert.AreSame(decrypted, cipherText);
        }

        [Test]
        public void RejectInvalidKeys()
        {
            var invalidKeys = new string[] { String.Empty, " ", "abcde" };

            foreach (var invalidKey in invalidKeys)
            {
                var ex = Assert.Throws<ArgumentException>(() => L64.Encrypt(String.Empty, invalidKey));

                Assert.AreEqual(ex.Message, $"Invalid key: '{invalidKey}'. Expected a shuffled version of '+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'");
            }
        }
    }
}