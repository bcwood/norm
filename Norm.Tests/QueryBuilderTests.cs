﻿using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class QueryBuilderTests
    {
        [Test]
        public void QueryBuilder_NoParameters()
        {
            var qb = new QueryBuilder<Person>();

            Assert.AreEqual(0, qb.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person]", qb.ToSqlString());
        }

        #region Int Param

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_Equal()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id == 1);

            Assert.AreEqual(1, qb.Parameters.Count);
            Assert.IsTrue(qb.Parameters.ContainsKey("Id"));
            Assert.AreEqual(1, qb.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_LessThan()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id < 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] < @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_LessThanOrEqual()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id <= 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <= @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_GreaterThan()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id > 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] > @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_GreaterThanOrEqual()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id >= 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] >= @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_NotEqual()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id != 1);

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <> @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_Equal_VariableParameter()
        {
            int id = 1;
            var qb = new QueryBuilder<Person>().Where(p => p.Id == id);

            Assert.AreEqual(1, qb.Parameters.Count);
            Assert.IsTrue(qb.Parameters.ContainsKey("Id"));
            Assert.AreEqual(id, qb.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", qb.ToSqlString());
        }

        [Test]
        [Category("Int")]
        public void QueryBuilder_Int_Equal_MemberParameter()
        {
            var person = new Person { Id = 1 };
            var qb = new QueryBuilder<Person>().Where(p => p.Id == person.Id);

            Assert.AreEqual(1, qb.Parameters.Count);
            Assert.IsTrue(qb.Parameters.ContainsKey("Id"));
            Assert.AreEqual(person.Id, qb.Parameters["Id"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id", qb.ToSqlString());
        }

        #endregion // Int Param

        #region String Param

        [Test]
        [Category("String")]
        public void QueryBuilder_String_Equal()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName == "John");

            Assert.AreEqual(1, qb.Parameters.Count);
            Assert.IsTrue(qb.Parameters.ContainsKey("FirstName"));
            Assert.AreEqual("John", qb.Parameters["FirstName"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] = @FirstName", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_NotEqual()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName != "John");

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] <> @FirstName", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_StartsWith()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName.StartsWith("J"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE @FirstName + '%'", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_NotStartsWith()
        {
            var qb = new QueryBuilder<Person>().Where(p => !p.FirstName.StartsWith("J"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE @FirstName + '%')", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_EndsWith()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName.EndsWith("n"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE '%' + @FirstName", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_NotEndsWith()
        {
            var qb = new QueryBuilder<Person>().Where(p => !p.FirstName.EndsWith("n"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE '%' + @FirstName)", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_Contains()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName.Contains("o"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] LIKE '%' + @FirstName + '%'", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_NotContains()
        {
            var qb = new QueryBuilder<Person>().Where(p => !p.FirstName.Contains("o"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([FirstName] LIKE '%' + @FirstName + '%')", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_IsNullOrEmpty()
        {
            var qb = new QueryBuilder<Person>().Where(p => string.IsNullOrEmpty(p.FirstName));

            Assert.AreEqual(0, qb.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person] WHERE ([FirstName] IS NULL OR [FirstName] = '')", qb.ToSqlString());
        }

        [Test]
        [Category("String")]
        public void QueryBuilder_String_EqualsNull()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.FirstName == null);

            Assert.AreEqual(0, qb.Parameters.Count);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [FirstName] IS NULL", qb.ToSqlString());
        }

        #endregion // String Param

        #region Multi Param

        [Test]
        [Category("Multi")]
        public void QueryBuilder_MultiParam()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id == 1 && p.FirstName == "John");

            Assert.AreEqual(2, qb.Parameters.Count);
            Assert.IsTrue(qb.Parameters.ContainsKey("Id"));
            Assert.AreEqual(1, qb.Parameters["Id"]);
            Assert.IsTrue(qb.Parameters.ContainsKey("FirstName"));
            Assert.AreEqual("John", qb.Parameters["FirstName"]);
            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id AND [FirstName] = @FirstName", qb.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void QueryBuilder_MultiParam_StartsWith()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id == 1 && p.FirstName.StartsWith("John"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] = @Id AND [FirstName] LIKE @FirstName + '%'", qb.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void QueryBuilder_MultiParam_NotEqual()
        {
            var qb = new QueryBuilder<Person>().Where(p => p.Id != 1 || p.FirstName != "John");

            Assert.AreEqual("SELECT * FROM [Person] WHERE [Id] <> @Id OR [FirstName] <> @FirstName", qb.ToSqlString());
        }

        [Test]
        [Category("Multi")]
        public void QueryBuilder_MultiParam_OuterNot()
        {
            var qb = new QueryBuilder<Person>().Where(p => !(p.Id == 1 && p.FirstName == "John"));

            Assert.AreEqual("SELECT * FROM [Person] WHERE NOT ([Id] = @Id AND [FirstName] = @FirstName)", qb.ToSqlString());
        }

        #endregion // Multi Param
    }
}
