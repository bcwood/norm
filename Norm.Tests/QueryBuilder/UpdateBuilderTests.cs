using System;
using NUnit.Framework;
using Norm.QueryBuilder;

namespace Norm.Tests.QueryBuilder
{
    [TestFixture]
    public class UpdateBuilderTests
    {
        [Test]
        public void Update_Person()
        {
            var person = new Person
            {
                Id = 1,
                FirstName = "John",
                MiddleInitial = null,
                LastName = "Smith",
                Gender = 'M'
            };

            var query = new UpdateBuilder(person);

            Assert.AreEqual(6, query.Parameters.Count);
            Assert.AreEqual(person.Id, query.Parameters["Id"]);
            Assert.AreEqual(person.FirstName, query.Parameters["FirstName"]);
            Assert.AreEqual(DBNull.Value, query.Parameters["MiddleInitial"]);
            Assert.AreEqual(person.LastName, query.Parameters["LastName"]);
            Assert.AreEqual(person.Gender, query.Parameters["Gender"]);
            Assert.IsTrue(query.Parameters.ContainsKey("UpdateDate"));
            Assert.AreEqual("UPDATE [Person] SET [FirstName]=@FirstName,[MiddleInitial]=@MiddleInitial,[LastName]=@LastName,[Gender]=@Gender,[UpdateDate]=@UpdateDate WHERE [Id]=@Id", query.ToSqlString());
        }
    }
}
