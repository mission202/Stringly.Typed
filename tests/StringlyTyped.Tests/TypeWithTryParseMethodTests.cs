using System;
using NUnit.Framework;

namespace StringlyTyped.Tests
{
    [TestFixture]
    public class TypeWithTryParseMethodTests
    {
        [Test]
        public void ToStringlyTyped_ClassWithCustomParseMethod_ParsesString()
        {
            UppercasedString actual = GetStringly("Nyan Cat");

            Assert.That(actual.Value, Is.EqualTo("NYAN CAT"));
        }

        [Test]
        public void ToString_ClassWithCustomParseMethod_ReturnsOriginalValue()
        {
            var expected = "Nyan Cat";

            var actual = GetStringly(expected);

            Assert.That(actual.ToString(), Is.EqualTo(expected));
        }

        private Stringly<UppercasedString> GetStringly(string value)
        {
            return new Stringly<UppercasedString>(value);
        }

        [Test]
        public void Ctor_TypedThatThrows_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { Stringly<TypeWithTypeParseThatThrows> value = "test"; });
        }
    }

    internal class UppercasedString
    {
        public readonly string Value;

        private UppercasedString(string value)
        {
            Value = value.ToUpperInvariant();
        }

        public static bool TryParse(string value, out UppercasedString result)
        {
            result = new UppercasedString(value);
            return true;
        }
    }

    internal class TypeWithTypeParseThatThrows
    {
        public static bool TryParse(string value, out TypeWithTypeParseThatThrows result)
        {
            throw new Exception("KA BOOM!");
        }
    }
}