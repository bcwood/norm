using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Norm.Query;

namespace Norm
{
    public class Database : IDisposable
    {
        private IDbConnection _connection;
        private bool _isConnectionOpener;

        /// <summary>
        /// Initializes a Database object. Opens a connection to the database using the supplied <paramref name="connectionString"/>.
        /// The database connection will be closed when the Database object is disposed.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public Database(string connectionString)
        {
            // TODO: initialize _connection based on provider type
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _isConnectionOpener = true;
        }

        /// <summary>
        /// Initializes a Database object. Assumes that the database <paramref name="connection"/> is already open,
        /// and leaves it open with the Database object is disposed.
        /// </summary>
        /// <param name="connection"></param>
        public Database(IDbConnection connection)
        {
            _connection = connection;
            _isConnectionOpener = false;
        }

        public void Dispose()
        {
            if (_isConnectionOpener)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        /// <summary>
        /// Executes a select query on type T, using the optional <paramref name="where"/> expression.
        /// </summary>
        /// <typeparam name="T">Type to query.</typeparam>
        /// <param name="where">Expression for querying the type T (optional).</param>
        /// <returns>Returns a SelectQuery object, which provides a fluent interface for fine-tuning your SELECT query.</returns>
        public SelectQuery<T> Select<T>(Expression<Func<T, bool>> where = null) where T : new()
        {
            return new SelectQuery<T>(_connection, where);
        }

        /// <summary>
        /// Inserts a new object into the database.
        /// </summary>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Returns the primary key of the object.</returns>
        public int Insert(object obj)
        {
            var insert = new InsertQuery(_connection, obj);

            return Convert.ToInt32(insert.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing object in the database.
        /// </summary>
        /// <param name="obj">The object to update.</param>
        /// <returns>Returns true if the object is updated successfully, otherwise false.</returns>
        public bool Update(object obj)
        {
            var update = new UpdateQuery(_connection, obj);

            return update.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Deletes an existing object from the database.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        /// <returns>Returns true if the object is deleted successfully, otherwise false.</returns>
        public bool Delete(object obj)
        {
            var delete = new DeleteQuery(_connection, obj);

            return delete.ExecuteNonQuery() == 1;
        }
    }
}
