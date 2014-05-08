using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Norm.QueryBuilder;

namespace Norm.Query
{
    public class SelectQuery<T> : BaseQuery where T : new()
    {
        internal SelectQuery(IDbConnection connection, Expression<Func<T, bool>> where = null)
            : base(connection)
        {
            base.QueryBuilder = new SelectBuilder<T>(where);
        }

        private SelectBuilder<T> SelectBuilder
        {
            get { return base.QueryBuilder as SelectBuilder<T>; }
        }

        /// <summary>
        /// Limits the max number of records to return.
        /// </summary>
        /// <param name="count">Max number of records to return</param>
        /// <returns>SelectBuilder object.</returns>
        public SelectQuery<T> Limit(int count)
        {
            this.SelectBuilder.Limit(count);
            return this;
        }

        /// <summary>
        /// Orders the query results by the given <paramref name="orderBy"/> expression.
        /// </summary>
        /// <param name="orderBy">The field to order by.</param>
        /// <returns>SelectBuilder object.</returns>
        public SelectQuery<T> OrderBy(Expression<Func<T, object>> orderBy)
        {
            this.SelectBuilder.OrderBy(orderBy);
            return this;
        }

        /// <summary>
        /// Orders the query results by the given <paramref name="orderBy"/> expression, in descending order.
        /// </summary>
        /// <param name="orderBy">The field to order by.</param>
        /// <returns>SelectBuilder object.</returns>
        public SelectQuery<T> OrderByDesc(Expression<Func<T, object>> orderByDesc)
        {
            this.SelectBuilder.OrderByDesc(orderByDesc);
            return this;
        }

        /// <summary>
        /// Returns the only object matching the query, or null if there are no matching objects. Throws an exception if there is more than one matching object.
        /// </summary>
        /// <returns>The object.</returns>
        public T SingleOrDefault()
        {
            return this.ToList().SingleOrDefault();
        }

        /// <summary>
        /// Returns a list of objects matching the query.
        /// </summary>
        /// <returns>The list of objects.</returns>
        public IEnumerable<T> ToList()
        {
            IDataReader reader = base.ExecuteReader();
            return this.MapReader<T>(reader);
        }

        private IEnumerable<T> MapReader<T>(IDataReader reader) where T : new()
        {
            var list = new List<T>();

            while (reader.Read())
            {
                T obj = new T();
                Type type = obj.GetType();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string propertyName = reader.GetName(i);
                    PropertyInfo property = type.GetCachedProperty(propertyName);

                    if (property != null)
                    {
                        object value = TypeConverter.ConvertToType(reader[propertyName], property.PropertyType);
                        property.SetValue(obj, value, null);
                    }
                }

                list.Add(obj);
            }

            reader.Close();

            return list;
        }
    }
}
