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
        public void GetPrimaryKey_PersonNoPk_ThrowsException()
        {
            Type type = typeof(PersonNoPk);
            
            Assert.Throws<Exception>(() => type.GetPrimaryKey());
        }

        #endregion // GetPrimaryKey
    }
}
