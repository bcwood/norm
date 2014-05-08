using System;
using NUnit.Framework;
using Norm.QueryBuilder;

namespace Norm.Tests.QueryBuilder
{
    [TestFixture]
    public class InsertBuilderTests
    {
        [Test]
        public void Insert_Person()
        {
            var person = new Person
            {
                FirstName = "John",
                MiddleInitial = null,
                LastName = "Smith",
                Gender = 'M'
            };

            var query = new InsertBuilder(person);

            Assert.AreEqual(5, query.Parameters.Count);
            Assert.AreEqual(person.FirstName, query.Parameters["FirstName"]);
            Assert.AreEqual(DBNull.Value, query.Parameters["MiddleInitial"]);
            Assert.AreEqual(person.LastName, query.Parameters["LastName"]);
            Assert.AreEqual(person.Gender, query.Parameters["Gender"]);
            Assert.IsTrue(query.Parameters.ContainsKey("CreateDate"));
            Assert.AreEqual("INSERT INTO [Person] ([FirstName],[MiddleInitial],[LastName],[Gender],[CreateDate]) VALUES (@FirstName,@MiddleInitial,@LastName,@Gender,@CreateDate); SELECT SCOPE_IDENTITY();", query.ToSqlString());
        }
    }
}
