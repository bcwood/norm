using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Norm.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
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
    }
}
