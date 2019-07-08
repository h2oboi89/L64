using NUnit.Framework;
using Cryptography;

namespace Tests
{
    public class Extensions_ShuffleShould
    {
        [Test]
        public void ReturnShuffledString()
        {
            var input = "Hello, World!";
            var result = input.Shuffle();

            Assert.AreNotEqual(input, result);
            Assert.AreNotSame(result, input);
        }
    }
}