using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Norm.QueryBuilder;

namespace Norm
{
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Executes a select query on type T, using the optional <paramref name="where"/>.
        /// </summary>
        /// <typeparam name="T">Type to query.</typeparam>
        /// <param name="connection">The database connection, which this method expects to already be open.</param>
        /// <param name="where">Expression for querying the type T (optional).</param>
        /// <param name="orderBy">Expression for field to order by (optional).</param>
        /// <param name="direction">Direction to sort (optional).</param>
        /// <param name="top">Top # of records to return (optional).</param>
        /// <returns>Returns an enumerable list of T objects matching the select criteria.</returns>
        public static IEnumerable<T> Select<T>(this IDbConnection connection, Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, SortDirection direction = SortDirection.Asc, int? top = null) where T : new()
        {
            var queryBuilder = new SelectBuilder<T>();
            IDbCommand command = connection.CreateCommand();
            
            if (where != null)
            {
                queryBuilder.Where(where);
                LoadParameters(command, queryBuilder.Parameters);
            }

            if (orderBy != null)
                queryBuilder.OrderBy(orderBy, direction);

            if (top != null)
                queryBuilder.Top(top.Value);

            command.CommandText = queryBuilder.ToSqlString();

            return command.ExecuteReader().Map<T>();
        }

        /// <summary>
        /// Inserts a new object into the database.
        /// </summary>
        /// <param name="connection">The database connection, which this method expects to already be open.</param>
        /// <param name="obj">Object to insert.</param>
        /// <returns>Returns the primary key of the object.</returns>
        public static int Insert(this IDbConnection connection, object obj)
        {
            var queryBuilder = new InsertBuilder(obj);

            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryBuilder.ToSqlString();
            LoadParameters(command, queryBuilder.Parameters);
            
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Updates an existing object in the database.
        /// </summary>
        /// <param name="connection">The database connection, which this method expects to already be open.</param>
        /// <param name="obj">The object to update.</param>
        /// <returns>Returns true if the object is updated successfully, otherwise false.</returns>
        public static bool Update(this IDbConnection connection, object obj)
        {
            var queryBuilder = new UpdateBuilder(obj);

            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryBuilder.ToSqlString();
            LoadParameters(command, queryBuilder.Parameters);
            
            return command.ExecuteNonQuery() == 1;
        }

        /// <summary>
        /// Deletes an existing object from the database.
        /// </summary>
        /// <param name="connection">The database connection, which this method expects to already be open.</param>
        /// <param name="obj">The object to delete.</param>
        /// <returns>Returns true if the object is deleted successfully, otherwise false.</returns>
        public static bool Delete(this IDbConnection connection, object obj)
        {
            var queryBuilder = new DeleteBuilder(obj);

            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryBuilder.ToSqlString();
            LoadParameters(command, queryBuilder.Parameters);

            return command.ExecuteNonQuery() == 1;
        }

        private static void LoadParameters(IDbCommand command, Dictionary<string, object> parameters)
        {
            command.Parameters.Clear();

            foreach (string key in parameters.Keys)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = key;
                parameter.Value = parameters[key];

                command.Parameters.Add(parameter);
            }
        }
    }
}
