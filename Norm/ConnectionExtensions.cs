using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Norm
{
    public static class ConnectionExtensions
    {
        private static List<string> _createdFields = new List<string> { "created", "createdate", "createdon" };
        private static List<string> _updatedFields = new List<string> { "updated", "updatedate", "updatedon" };

        /// <summary>
        /// Executes a select query on type T, using the optional <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="T">Type to query.</typeparam>
        /// <param name="connection">The database connection, which this method expects to already be open.</param>
        /// <param name="expression">Optional Expression for querying the type T.</param>
        /// <returns>Returns an enumerable list of T objects matching the select criteria.</returns>
        public static IEnumerable<T> Select<T>(this IDbConnection connection, Expression<Func<T, bool>> expression = null) where T : new()
        {
            IDbCommand command = connection.CreateCommand();
            var queryBuilder = new QueryBuilder<T>();

            if (expression != null)
            {
                queryBuilder.Where(expression);

                foreach (string key in queryBuilder.Parameters.Keys)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = key;
                    parameter.Value = queryBuilder.Parameters[key];

                    command.Parameters.Add(parameter);
                }
            }

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
            IDbCommand command = connection.CreateCommand();

            var fieldList = new List<string>();
            var valueList = new List<string>();

            Type type = obj.GetType();
            PropertyInfo primaryKey = type.GetPrimaryKey();

            if (primaryKey == null)
                throw new Exception(string.Format("Primary key field not found for type {0} (Id or {0}Id expected).", type.Name));

            var excludedFields = new List<string>();
            excludedFields.AddRange(_updatedFields);
            excludedFields.Add(primaryKey.Name.ToLower());

            foreach (PropertyInfo property in type.GetCachedProperties())
            {
                if (excludedFields.Contains(property.Name.ToLower()))
                {
                    continue;
                }

                fieldList.Add("[" + property.Name + "]");
                valueList.Add("@" + property.Name);

                var p = command.CreateParameter();
                p.ParameterName = property.Name;

                // set created field
                if (_createdFields.Contains(property.Name.ToLower()))
                {
                    p.Value = DateTime.Now;
                    property.SetValue(obj, p.Value, null);
                }
                else
                {
                    object value = property.GetValue(obj, null);

                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        p.Value = DBNull.Value;
                    else
                        p.Value = value;
                }

                command.Parameters.Add(p);
            }

            string query = string.Format("insert into [{0}] ({1}) output inserted.[{2}] values ({3})",
                                          type.Name, string.Join(",", fieldList), primaryKey.Name, string.Join(",", valueList));

            command.CommandText = query;

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
            IDbCommand command = connection.CreateCommand();
            var setList = new List<string>();

            Type type = obj.GetType();
            PropertyInfo primaryKey = type.GetPrimaryKey();

            if (primaryKey == null)
                throw new Exception(string.Format("Primary key field not found for type {0} (Id or {0}Id expected).", type.Name));

            var excludedFields = new List<string>();
            excludedFields.AddRange(_createdFields);

            foreach (PropertyInfo property in type.GetCachedProperties())
            {
                if (excludedFields.Contains(property.Name.ToLower()))
                {
                    continue;
                }

                if (property.Name != primaryKey.Name)
                {
                    setList.Add(string.Format("[{0}] = @{0}", property.Name));
                }

                var p = command.CreateParameter();
                p.ParameterName = property.Name;

                // set updated field
                if (_updatedFields.Contains(property.Name.ToLower()))
                {
                    p.Value = DateTime.Now;
                    property.SetValue(obj, p.Value, null);
                }
                else
                {
                    object value = property.GetValue(obj, null);

                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        p.Value = DBNull.Value;
                    else
                        p.Value = value;
                }

                command.Parameters.Add(p);
            }

            string query = string.Format("update [{0}] set {1} where [{2}] = @{2}",
                                          type.Name, string.Join(",", setList), primaryKey.Name);

            command.CommandText = query;

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
            IDbCommand command = connection.CreateCommand();

            Type type = obj.GetType();
            PropertyInfo primaryKey = type.GetPrimaryKey();

            if (primaryKey == null)
                throw new Exception(string.Format("Primary key field not found for type {0} (Id or {0}Id expected).", type.Name));

            var parameter = command.CreateParameter();
            parameter.ParameterName = primaryKey.Name;
            parameter.Value = primaryKey.GetValue(obj, null);
            command.Parameters.Add(parameter);

            string query = string.Format("delete from [{0}] where [{1}] = @{1}",
                                          type.Name, primaryKey.Name);

            command.CommandText = query;

            return command.ExecuteNonQuery() == 1;
        }
    }
}
