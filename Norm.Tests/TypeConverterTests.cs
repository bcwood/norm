using System;
using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class TypeConverterTests
    {
        [TestCase(typeof(String), TestName = "String")]
        [TestCase(typeof(Int32), TestName = "Int32")]
        [TestCase(typeof(Int32?), TestName = "Int32?")]
        [TestCase(typeof(Decimal), TestName = "Decimal")]
        [TestCase(typeof(Decimal?), TestName = "Decimal?")]
        [TestCase(typeof(Double), TestName = "Double")]
        [TestCase(typeof(Double?), TestName = "Double?")]
        [TestCase(typeof(Boolean), TestName = "Boolean")]
        [TestCase(typeof(Char), TestName = "Char")]
        [TestCase(typeof(Char?), TestName = "Char?")]
        [TestCase(typeof(Guid), TestName = "Guid")]
        [TestCase(typeof(DateTime), TestName = "DateTime")]
        [TestCase(typeof(DateTime?), TestName = "DateTime?")]
        [Category("TypeConversion")]
        public void ConvertToType_NullValue_ReturnsNull(Type type)
        {
            Assert.IsNull(TypeConverter.ConvertToType(null, type));
        }

        [TestCase("abcde", typeof(String), ExpectedResult = "abcde", TestName = "String")]
        [TestCase("123", typeof(Int32), ExpectedResult = 123, TestName = "Int32")]
        [TestCase("123", typeof(Int32?), ExpectedResult = 123, TestName = "Int32?")]
        [TestCase("123.4", typeof(Decimal), ExpectedResult = 123.4, TestName = "Decimal")]
        [TestCase("123.4", typeof(Decimal?), ExpectedResult = 123.4, TestName = "Decimal?")]
        [TestCase("123.4", typeof(Double), ExpectedResult = 123.4, TestName = "Double")]
        [TestCase("123.4", typeof(Double?), ExpectedResult = 123.4, TestName = "Double?")]
        [TestCase("true", typeof(Boolean), ExpectedResult = true, TestName = "Boolean")]
        [TestCase("A", typeof(Char), ExpectedResult = 'A', TestName = "Char")]
        [TestCase("A", typeof(Char?), ExpectedResult = 'A', TestName = "Char?")]
        [Category("TypeConversion")]
        public object ConvertToType_ReturnsCorrectlyConvertedType(object value, Type type)
        {
            return TypeConverter.ConvertToType(value, type);
        }

        // testing of specific types that doesn't lend themselves to the previous generic tests

        [Test]
        [Category("TypeConversion")]
        public void ConvertToGuid_ReturnsEqualGuid()
        {
            object g1 = "79b4a64f-100a-43c7-9d57-12ba0342b449";

            object g2 = TypeConverter.ConvertToType(g1, typeof(Guid));

            Assert.AreEqual(g2, new Guid(g1.ToString()));
        }

        [Test]
        [Category("TypeConversion")]
        public void ConvertToDateTime_ReturnsEqualDateTime()
        {
            object d1 = "1/2/2013";

            object d2 = TypeConverter.ConvertToType(d1, typeof(DateTime));

            Assert.AreEqual(d2, DateTime.Parse(d1.ToString()));
        }

        [Test]
        [Category("TypeConversion")]
        public void ConvertToNullableDateTime_ReturnsEqualDateTime()
        {
            object d1 = "1/2/2013";

            object d2 = TypeConverter.ConvertToType(d1, typeof(DateTime?));

            Assert.AreEqual(d2, DateTime.Parse(d1.ToString()));
        }
    }
}
