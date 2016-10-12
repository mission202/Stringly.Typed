using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace StringlyTyped.Tests
{
    [TestFixture]
    public class Samples
    {
        private SampleService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new SampleService();
        }

        [Test]
        public void Service_MethodWithPrimitives_WorksWithValidStrings()
        {
            // "Just works" as primitives have a static "TryParse" method.
            Assert.DoesNotThrow(() => _service.MethodWithPrimitives(
                "string",
                new Stringly<int>("1"),
                new Stringly<Guid>("fbaabebf-aa2e-4ea8-9b81-91eb1e174926"),
                new Stringly<bool>("FALSE")));
        }

        [Test]
        public void Service_MethodWithInt_ThrowsWithInvalidString()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.Method(new Stringly<int>("one")));
        }

        [Test]
        public void Service_MethodWithGuid_ThrowsWithInvalidString()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.Method(new Stringly<Guid>("im-not-a-guid-sadface")));
        }

        [Test]
        public void Service_MethodWithBoolean_ThrowsWithInvalidString()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.Method(new Stringly<bool>("no")));
        }

        [Test]
        public void MethodWithStringlyParam_GivenStringlyTyped_ValidatesCorrectly()
        {
            Assert.DoesNotThrow(
                () => _service.DoDatabaseStuff(new Stringly<RecordId>("123")));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.DoDatabaseStuff(new Stringly<RecordId>("some-text")));
        }

        [Test]
        public void MethodWithStringlyTypedParam_GivenAString_ValidatesCorrectly()
        {
            Assert.DoesNotThrow(
                () => _service.DontWantToNewStringly("123"));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => _service.DontWantToNewStringly("fail"));
        }

        [Test]
        public void StringlyComplexType_GivenAString_ValidatesAndCreatesTypeCorrectly()
        {
            Assert.That(_service.CalculateArea(new Stringly<Size>("10x10")), Is.EqualTo(100));

            Size size = new Stringly<Size>("42x1337");
            Assert.That(size.Width, Is.EqualTo(42));
            Assert.That(size.Height, Is.EqualTo(1337));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Stringly<Size>("w:10 h:10"));
        }
    }

    internal class SampleService
    {
        internal void MethodWithPrimitives(string s, int i, Guid g, bool b) { }

        internal void Method(int i) { }
        internal void Method(Guid g) { }
        internal void Method(bool b) { }

        internal void DoDatabaseStuff(RecordId recordId) { }

        internal void DontWantToNewStringly(Stringly<RecordId> recordId)
        {
            RecordId id = recordId;
        }

        internal int CalculateArea(Size size)
        {
            return size.Width * size.Height;
        }
    }

    internal class RecordId : Stringly
    {
        protected override Regex Regex => new Regex(@"^\d+$");
    }

    internal class Size : StringlyPattern<Size>
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        protected override Regex Regex => new Regex(@"^(?<width>\d+)x(?<height>\d+)$");

        protected override Size ParseFromRegex(Match match)
        {
            return new Size
            {
                Width = int.Parse(match.Groups["width"].Value),
                Height = int.Parse(match.Groups["height"].Value)
            };
        }
    }
}