using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class ConnectionExtensionTests
    {
        private const int VALID_ID = 1;
        private const int INVALID_ID = 99999;

        private SqlConnection _connection;
        
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Norm.Tests"].ConnectionString);
            _connection.Open();
        }

        [TearDown]
        public void Teardown()
        {
            _connection.Close();
            _connection.Dispose();
        }

        #endregion // Setup/Teardown

        #region Select

        [Test]
        [Category("Select")]
        public void Select_ById_ReturnsNotNull()
        {
            Person person = _connection.Select<Person>(p => p.Id == VALID_ID).SingleOrDefault();

            Assert.IsNotNull(person);
        }

        [Test]
        [Category("Select")]
        public void Select_ById_RecordNotExists_ReturnsNull()
        {
            Person person = _connection.Select<Person>(p => p.Id == INVALID_ID).SingleOrDefault();

            Assert.IsNull(person);
        }

        [Test]
        [Category("Select")]
        public void Select_All_ReturnsNonEmptyList()
        {
            IEnumerable<Person> list = _connection.Select<Person>();

            Assert.IsNotEmpty(list);
        }

        [Test]
        [Category("Select")]
        public void Select_Top5_ReturnsListOfLength5()
        {
            IEnumerable<Person> list = _connection.Select<Person>(top: 5);

            Assert.AreEqual(5, list.Count());
        }

        [Test]
        [Category("Select")]
        public void Select_ById_500Iterations()
        {
            for (int i = 1; i <= 500; i++)
            {
                int id = i;
                Person person = _connection.Select<Person>(p => p.Id == id).SingleOrDefault();

                Assert.IsNotNull(person);
            }
        }

        #endregion // Select

        #region Insert

        [Test]
        [Category("Insert")]
        public void Insert_ReturnsNonZeroId()
        {
            var person = Person.Random();
            person.Id = _connection.Insert(person);

            Assert.IsTrue(person.Id > 0);
        }

        [Test]
        [Category("Insert")]
        public void Insert_GetById_ReturnsEqualObject()
        {
            var person1 = Person.Random();
            person1.Id = _connection.Insert(person1);

            var p2 = _connection.Select<Person>(p => p.Id == person1.Id).SingleOrDefault();

            Assert.AreEqual(person1, p2);
        }

        #endregion // Insert

        #region Update

        [Test]
        [Category("Update")]
        public void Update_ReturnsTrue()
        {
            Person person = _connection.Select<Person>(p => p.Id == VALID_ID).SingleOrDefault();

            person.FirstName = TestHarness.GetRandomString(20);
            person.LastName = TestHarness.GetRandomString(20);

            Assert.IsTrue(_connection.Update(person));
        }

        [Test]
        [Category("Update")]
        public void Update_RecordNotExists_ReturnsFalse()
        {
            var person = Person.Random();
            person.Id = INVALID_ID;
            
            Assert.IsFalse(_connection.Update(person));
        }

        [Test]
        [Category("Update")]
        public void Update_GetById_ReturnsEqualObject()
        {
            Person person1 = _connection.Select<Person>(p => p.Id == VALID_ID).SingleOrDefault();

            person1.FirstName = TestHarness.GetRandomString(20);
            person1.LastName = TestHarness.GetRandomString(20);

            _connection.Update(person1);

            Person person2 = _connection.Select<Person>(p => p.Id == VALID_ID).SingleOrDefault();

            Assert.AreEqual(person1, person2);
        }

        #endregion // Update

        #region Delete

        [Test]
        [Category("Delete")]
        public void Delete_ReturnsTrue()
        {
            var person = Person.Random();
            person.Id = _connection.Insert(person);

            Assert.IsTrue(_connection.Delete(person));
        }

        [Test]
        [Category("Delete")]
        public void Delete_RecordNotExists_ReturnsFalse()
        {
            var person = Person.Random();
            person.Id = INVALID_ID;

            Assert.IsFalse(_connection.Delete(person));
        }

        [Test]
        [Category("Delete")]
        public void Delete_GetById_ReturnsNull()
        {
            var person1 = Person.Random();
            person1.Id = _connection.Insert(person1);
            _connection.Delete(person1);

            var person2 = _connection.Select<Person>(p => p.Id == person1.Id).SingleOrDefault();

            Assert.IsNull(person2);
        }

        #endregion // Delete
    }
}
