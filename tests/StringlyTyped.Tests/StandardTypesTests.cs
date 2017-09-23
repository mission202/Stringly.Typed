using System;
using NUnit.Framework;

namespace StringlyTyped.Tests
{
    [TestFixture]
    public class StringlyTests
    {
        [Test]
        public void Ctor_ValueIsGuid_SetsValue()
        {
            var expected = Guid.NewGuid().ToString();

            var value = new Stringly<Guid>(expected).Value;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void Ctor_ValueIsntGuid_ThrowsArgumentOutofRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Stringly<Guid>("this-is-not-a-guid"));
        }

        [Test]
        public void Ctor_ValueIsInt_SetsValue()
        {
            var expected = 42.ToString();

            var value = new Stringly<int>(expected).Value;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void Ctor_ValueIsntInt_ThrowsArgumentOutofRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Stringly<int>("this-is-not-a-int"));
        }

        [Test]
        public void ToString_BooleanValue_ReturnsStringPassed()
        {
            var expected = "FALSE";

            var result = new Stringly<bool>(expected).ToString();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ImplicitString_BooleanValue_ReturnsStringPassed()
        {
            var expected = "FALSE";

            string result = new Stringly<bool>(expected);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void StaticParse_MatchingValue_ReturnsParsedValue()
        {
            Assert.True(Stringly<bool>.Parse("true"));
        }

        [Test]
        public void StaticParse_NonMatchingValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Stringly<bool>.Parse("woops"));
        }

        [Test]
        public void MethodWithStringlyTypedInt_IntAsString_ReturnsIntValue()
        {
            var result = Increment("1");

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void MethodWithStringlyTypedInt_IntAsItself_ReturnsIntValue()
        {
            var result = Increment(1);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void MethodWithStringlyTypedInt_InvalidIntAsString_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Increment("WTF"));
        }

        public static int Increment(Stringly<int> digits)
        {
            return digits + 1;
        }
    }
}