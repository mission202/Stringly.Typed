using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace StringlyTyped.Tests
{
    [TestFixture]
    public class SimpleStringCustomTypeTest
    {
        [Test]
        public void StringlyType_MethodWithConcreteParam_CreatesObject()
        {
            const string expected = "SPACECAT";

            Action<LettersOnly> func = stringly =>
            {
                Assert.That(stringly.ToString(), Is.EqualTo(expected));
            };

            func(new Stringly<LettersOnly>(expected));
        }

        [Test]
        public void StringlyType_MethodWithConcreteParam_ThrowsArgumentOutOfRangeException()
        {
            Func<LettersOnly, string> func = stringly => stringly.ToString();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => func(new Stringly<LettersOnly>("12345")));
        }
    }

    internal class LettersOnly : Stringly
    {
        protected override Regex Regex => new Regex(@"^[a-zA-Z]+$");
    }
}