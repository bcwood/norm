using NUnit.Framework;
using Norm.QueryBuilder;

namespace Norm.Tests.QueryBuilder
{
    [TestFixture]
    public class DeleteBuilderTests
    {
        [Test]
        public void Delete_Person()
        {
            var person = new Person { Id = 1 };
            var query = new DeleteBuilder(person);

            Assert.AreEqual(1, query.Parameters.Count);
            Assert.AreEqual(person.Id, query.Parameters["Id"]);
            Assert.AreEqual("DELETE FROM [Person] WHERE [Id]=@Id", query.ToSqlString());
        }
    }
}
