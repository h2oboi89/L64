using Cryptography;
using NUnit.Framework;
using System.Security.Cryptography;

namespace UnitTests.Cryptography
{
    [TestFixture]
    public class ExtensionsShould
    {
        [Test]
        public void ShuffleShouldShuffleString()
        {
            var input = "Hello, World!";
            var result = input.Shuffle();

            Assert.That(input, Is.Not.EqualTo(result));
            Assert.That(result, Is.Not.SameAs(input));
        }

        [Test] 
        public void ShuffleNullOrEmptyReturnsEmptyString()
        {
            Assert.That(string.Empty.Shuffle(), Is.EqualTo(string.Empty));
            Assert.That(((string)null).Shuffle(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetNextInt32ThrowsException()
        {
            RNGCryptoServiceProvider rnd = null;

            Assert.That(() => rnd.GetNextInt32(), Throws.ArgumentNullException);
        }
    }
}