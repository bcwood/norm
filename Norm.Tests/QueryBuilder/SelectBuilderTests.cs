using NUnit.Framework;
using Norm.QueryBuilder;

namespace Norm.Tests.QueryBuilder
{
    [TestFixture]
    public class SelectBuilderTests
    {
        [Test]
        public void Select_NoParameters()
        {
            var query = new SelectBuilder<Person>();

            Assert.AreEqual(0, query.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person]", query.ToSqlString());
        }

        #region Int Param

        [Test]
        [Category("Int")]
        public void Select_IntParam_Equal()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id == 1);

            Assert.AreEqual(1, query.Parameters.Count);
            Assert.AreEqual(1, query.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_NotEqual()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id != 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <> @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_LessThan()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id < 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] < @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_LessThanOrEqual()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id <= 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <= @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_GreaterThan()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id > 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] > @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_GreaterThanOrEqual()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id >= 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] >= @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_LocalVar_Equal()
        {
            int id = 1;
            var query = new SelectBuilder<Person>().Where(p => p.Id == id);

            Assert.AreEqual(1, query.Parameters.Count);
            Assert.AreEqual(id, query.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", query.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void Select_IntParam_MemberVar_Equal()
        {
            var person = new Person { Id = 1 };
            var query = new SelectBuilder<Person>().Where(p => p.Id == person.Id);

            Assert.AreEqual(1, query.Parameters.Count);
            Assert.AreEqual(person.Id, query.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", query.ToSqlString());
        }

        #endregion // Int Param

        #region String Param

        [Test]
        [Category("String")]
        public void Select_StringParam_Equal()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName == "John");

            Assert.AreEqual(1, query.Parameters.Count);
            Assert.AreEqual("John", query.Parameters["FirstName"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] = @FirstName", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_NotEqual()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName != "John");

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] <> @FirstName", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_StartsWith()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName.StartsWith("J"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE @FirstName + '%'", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_NotStartsWith()
        {
            var query = new SelectBuilder<Person>().Where(p => !p.FirstName.StartsWith("J"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE @FirstName + '%')", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_EndsWith()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName.EndsWith("n"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE '%' + @FirstName", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_NotEndsWith()
        {
            var query = new SelectBuilder<Person>().Where(p => !p.FirstName.EndsWith("n"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE '%' + @FirstName)", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_Contains()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName.Contains("o"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE '%' + @FirstName + '%'", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_NotContains()
        {
            var query = new SelectBuilder<Person>().Where(p => !p.FirstName.Contains("o"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE '%' + @FirstName + '%')", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_IsNullOrEmpty()
        {
            var query = new SelectBuilder<Person>().Where(p => string.IsNullOrEmpty(p.FirstName));

            Assert.AreEqual(0, query.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person] WHERE ([FirstName] IS NULL OR [FirstName] = '')", query.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void Select_StringParam_EqualsNull()
        {
            var query = new SelectBuilder<Person>().Where(p => p.FirstName == null);

            Assert.AreEqual(0, query.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] IS NULL", query.ToSqlString());
        }

        #endregion // String Param

        #region Multi Param

        [Test]
        [Category("Multi")]
        public void Select_MultiParam_Equal()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id == 1 && p.FirstName == "John");

            Assert.AreEqual(2, query.Parameters.Count);
            Assert.AreEqual(1, query.Parameters["Id"]);
            Assert.AreEqual("John", query.Parameters["FirstName"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id AND [FirstName] = @FirstName", query.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void Select_MultiParam_StartsWith()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id == 1 && p.FirstName.StartsWith("John"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id AND [FirstName] LIKE @FirstName + '%'", query.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void Select_MultiParam_NotEqual()
        {
            var query = new SelectBuilder<Person>().Where(p => p.Id != 1 || p.FirstName != "John");

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <> @Id OR [FirstName] <> @FirstName", query.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void Select_MultiParam_OuterNot()
        {
            var query = new SelectBuilder<Person>().Where(p => !(p.Id == 1 && p.FirstName == "John"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([Id] = @Id AND [FirstName] = @FirstName)", query.ToSqlString());
        }

        #endregion // Multi Param

        #region OrderBy

        [Test]
        [Category("OrderBy")]
        public void Select_OrderBy_SingleField()
        {
            var query = new SelectBuilder<Person>().OrderBy(p => p.LastName);

            Assert.AreEqual("SELECT * FROM [Person] ORDER BY [LastName] ASC", query.ToSqlString());
        }

        [Test]
        [Category("OrderBy")]
        public void Select_OrderBy_MultipleFields()
        {
            var query = new SelectBuilder<Person>()
                                .OrderBy(p => p.LastName)
                                .OrderBy(p => p.FirstName);

            Assert.AreEqual("SELECT * FROM [Person] ORDER BY [LastName] ASC,[FirstName] ASC", query.ToSqlString());
        }

        [Test]
        [Category("OrderBy")]
        public void Select_OrderBy_MultipleFields_DifferentDirections()
        {
            var query = new SelectBuilder<Person>()
                                .OrderBy(p => p.LastName, SortDirection.Desc)
                                .OrderBy(p => p.FirstName);

            Assert.AreEqual("SELECT * FROM [Person] ORDER BY [LastName] DESC,[FirstName] ASC", query.ToSqlString());
        }

        #endregion // OrderBy
    }
}
