using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Norm
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Maps the results inside an IDataReader to a given type T.
        /// </summary>
        /// <typeparam name="T">Type of object to map results to.</typeparam>
        /// <param name="reader">IDataReader to map.</param>
        /// <returns>Returns an enumerable list of mapped T objects.</returns>
        internal static IEnumerable<T> Map<T>(this IDataReader reader) where T : new()
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
                        
                        property.SetValue(obj, value);
                    }
                }

                list.Add(obj);
            }

            reader.Close();

            return list;
        }
    }
}
