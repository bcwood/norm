using System;
using System.Reflection;
using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class TypeExtensionTests
    {
        #region GetPrimaryKey

        [Test]
        [Category("GetPrimaryKey")]
        public void GetPrimaryKey_Person_Returns_Id()
        {
            Type type = typeof(Person);
            PropertyInfo primaryKey = type.GetPrimaryKey();

            Assert.IsNotNull(primaryKey);
            Assert.AreEqual("Id", primaryKey.Name);
        }

        [Test]
        [Category("GetPrimaryKey")]
        public void GetPrimaryKey_PersonAlt_Returns_PersonAltId()
        {
            Type type = typeof(PersonAltPk);
            PropertyInfo primaryKey = type.GetPrimaryKey();

            Assert.IsNotNull(primaryKey);
            Assert.AreEqual("PersonAltPkId", primaryKey.Name);
        }

        [Test]
        [Category("GetPrimaryKey")]
        public void GetPrimaryKey_PersonNoPk_ReturnsNull()
        {
            Type type = typeof(PersonNoPk);
            PropertyInfo primaryKey = type.GetPrimaryKey();

            Assert.IsNull(primaryKey);
        }

        #endregion // GetPrimaryKey
    }
}
