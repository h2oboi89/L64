using Cryptography;
using NUnit.Framework;

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

            Assert.AreNotEqual(input, result);
            Assert.AreNotSame(result, input);
        }
    }
}