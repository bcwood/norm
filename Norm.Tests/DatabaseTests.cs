using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private const int VALID_ID = 1;
        private const int INVALID_ID = 99999;

        private Norm.Database _db;
        
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _db = new Norm.Database(ConfigurationManager.ConnectionStrings["Norm.Tests"].ConnectionString);
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        #endregion // Setup/Teardown

		#region Select

        [Test]
        [Category("Select")]
        public void Select_ById_ReturnsNotNull()
        {
            Person person = _db.Select<Person>(p => p.Id == VALID_ID)
                                .SingleOrDefault();

            Assert.IsNotNull(person);
        }

        [Test]
        [Category("Select")]
        public void Select_ById_RecordNotExists_ReturnsNull()
        {
            Person person = _db.Select<Person>(p => p.Id == INVALID_ID)
                                .SingleOrDefault();

            Assert.IsNull(person);
        }

        [Test]
        [Category("Select")]
        public void Select_All_ReturnsNonEmptyList()
        {
            IEnumerable<Person> list = _db.Select<Person>().ToList();

            Assert.IsNotEmpty(list);
        }

        [Test]
        [Category("Select")]
        public void Select_Top5_ReturnsListOfLength5()
        {
            IEnumerable<Person> list = _db.Select<Person>()
                                            .Limit(5)
                                            .ToList();

            Assert.AreEqual(5, list.Count());
        }

	    [Test]
	    [Category("Select")]
	    public void Select_OrderBy()
	    {
			List<Person> list = _db.Select<Person>()
											.OrderBy(p => p.Id)
											.ToList().ToList();

			for (int i = 0; i < list.Count - 1; i++)
			{
				Assert.Less(list[i].Id, list[i+1].Id);
			}
	    }

		[Test]
		[Category("Select")]
		public void Select_OrderByDesc()
		{
			List<Person> list = _db.Select<Person>()
											.OrderByDesc(p => p.Id)
											.ToList().ToList();

			for (int i = 0; i < list.Count - 1; i++)
			{
				Assert.Greater(list[i].Id, list[i + 1].Id);
			}
		}

        [Test]
        [Category("Select")]
        public void Select_ById_500Iterations()
        {
            for (int i = 1; i <= 500; i++)
            {
                int id = i;
                Person person = _db.Select<Person>(p => p.Id == id)
                                    .SingleOrDefault();

                Assert.IsNotNull(person);
            }
        }

		[Test]
		[Category("Select")]
		public void Select_UsingOpenConnection()
		{
			using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Norm.Tests"].ConnectionString))
			{
				conn.Open();

				var db = new Norm.Database(conn);
				Person person = db.Select<Person>(p => p.Id == VALID_ID)
									.SingleOrDefault();

				Assert.IsNotNull(person);

				conn.Close();
			}
		}

        #endregion // Select

        #region Insert

        [Test]
        [Category("Insert")]
        public void Insert_ReturnsNonZeroId()
        {
            var person = Person.Random();
            person.Id = _db.Insert(person);

            Assert.IsTrue(person.Id > 0);
        }

        [Test]
        [Category("Insert")]
        public void Insert_GetById_ReturnsEqualObject()
        {
            var person1 = Person.Random();
            person1.Id = _db.Insert(person1);

            var p2 = _db.Select<Person>(p => p.Id == person1.Id)
                        .SingleOrDefault();

            Assert.AreEqual(person1, p2);
        }

        #endregion // Insert

        #region Update

        [Test]
        [Category("Update")]
        public void Update_ReturnsTrue()
        {
            Person person = _db.Select<Person>(p => p.Id == VALID_ID)
                                .SingleOrDefault();

            person.FirstName = TestHarness.GetRandomString(20);
            person.LastName = TestHarness.GetRandomString(20);

            Assert.IsTrue(_db.Update(person));
        }

        [Test]
        [Category("Update")]
        public void Update_RecordNotExists_ReturnsFalse()
        {
            var person = Person.Random();
            person.Id = INVALID_ID;

            Assert.IsFalse(_db.Update(person));
        }

        [Test]
        [Category("Update")]
        public void Update_GetById_ReturnsEqualObject()
        {
            Person person1 = _db.Select<Person>(p => p.Id == VALID_ID)
                                .SingleOrDefault();

            person1.FirstName = TestHarness.GetRandomString(20);
            person1.LastName = TestHarness.GetRandomString(20);

            _db.Update(person1);

            Person person2 = _db.Select<Person>(p => p.Id == VALID_ID)
                                .SingleOrDefault();

            Assert.AreEqual(person1, person2);
        }

        #endregion // Update

        #region Delete

        [Test]
        [Category("Delete")]
        public void Delete_ReturnsTrue()
        {
            var person = Person.Random();
            person.Id = _db.Insert(person);

            Assert.IsTrue(_db.Delete(person));
        }

        [Test]
        [Category("Delete")]
        public void Delete_RecordNotExists_ReturnsFalse()
        {
            var person = Person.Random();
            person.Id = INVALID_ID;

            Assert.IsFalse(_db.Delete(person));
        }

        [Test]
        [Category("Delete")]
        public void Delete_GetById_ReturnsNull()
        {
            var person1 = Person.Random();
            person1.Id = _db.Insert(person1);
            _db.Delete(person1);

            var person2 = _db.Select<Person>(p => p.Id == person1.Id).SingleOrDefault();

            Assert.IsNull(person2);
        }

        #endregion // Delete
    }
}
