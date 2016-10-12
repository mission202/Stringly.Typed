using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace StringlyTyped.Tests
{
    public class TypeWithoutTryParseMethodTests
    {
        [Test]
        public void Ctor_GivenAString_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { Stringly<TypeWithoutTryParseMethod> cat = "nyan"; });
        }
    }

    internal class TypeWithoutTryParseMethod
    {

    }
}