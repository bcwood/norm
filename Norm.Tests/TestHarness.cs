using System;
using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Norm.Tests
{
    [SetUpFixture]
    public class TestHarness
    {
        #region Setup

        [SetUp]
        public void Setup()
        {
            string query = @"
                if (OBJECT_ID('Person') is null)
                    create table Person
	                (
		                Id int identity primary key, 
		                FirstName varchar(50) not null,
                        MiddleInitial char(1) null,
                        LastName varchar(50) not null,
                        Gender char(1) not null,
                        CreateDate datetime not null,
                        UpdateDate datetime null
	                )
                else
                    truncate table Person

	            while (select count(1) from Person) < 500
	            begin 		
		            insert into Person (FirstName, MiddleInitial, LastName, Gender, CreateDate) 
                    values (replicate('A', 50), 'B', replicate('C', 50), 'M', GETDATE())
	            end
                ";

            ExecuteQuery(query);
        }

        private void ExecuteQuery(string query)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Norm.Tests"].ConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        #endregion // Setup

        #region Helpers

        private static Random _random = new Random();

        public static string GetRandomString(int length)
        {
            string validChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] letters = new char[length];

            for (int i = 0; i < length; i++)
            {
                letters[i] = validChars[_random.Next(0, validChars.Length - 1)];
            }

            return new string(letters);
        }

        #endregion // Helpers
    }

    #region Test Models

    public class Person
    {
        // mapped directly to db fields
        public int Id { get; set; }
        public string FirstName { get; set; }
        public char? MiddleInitial { get; set; }
        public string LastName { get; set; }
        public char Gender { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public static Person Random()
        {
            var person = new Person();
            person.FirstName = TestHarness.GetRandomString(20);
            person.MiddleInitial = Convert.ToChar(TestHarness.GetRandomString(1));
            person.LastName = TestHarness.GetRandomString(20);
            person.Gender = 'M';

            return person;
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((Person)obj);
        }

        protected bool Equals(Person other)
        {
            return Id == other.Id &&
                   string.Equals(FirstName, other.FirstName) &&
                   MiddleInitial == other.MiddleInitial &&
                   string.Equals(LastName, other.LastName) &&
                   string.Equals(Gender, other.Gender);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MiddleInitial.GetHashCode();
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Gender.GetHashCode();
                return hashCode;
            }
        }

        #endregion // Equality
    }

    internal class PersonAltPk
    {
        // uses alternate primary key name

        public int PersonAltPkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    internal class PersonNoPk
    {
        // has no primary key

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    #endregion // Test Models
}
