using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norm
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Find the primary key for a given Type. 
        /// The field name is expected to be either Id or [TypeName]Id.
        /// </summary>
        /// <param name="type">The Type to get the primary key for.</param>
        /// <returns>Returns the primary key if found, otherwise throws an exception.</returns>
        public static PropertyInfo GetPrimaryKey(this Type type)
        {
            var primaryKeyFields = new List<string> { "id", type.Name.ToLower() + "id" };
            var primaryKey = type.GetCachedProperties().SingleOrDefault(p => primaryKeyFields.Contains(p.Name.ToLower()));

            if (primaryKey == null)
                throw new Exception(string.Format("Primary key field not found for type {0} (Id or {0}Id expected).", type.Name));

            return primaryKey;
        }

        private static Hashtable _propertyCache = new Hashtable();

        internal static PropertyInfo[] GetCachedProperties(this Type type)
        {
            string cacheKey = type.FullName;

            if (!_propertyCache.ContainsKey(cacheKey))
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propertyCache.Add(cacheKey, properties);
                
                return properties;
            }

            return (PropertyInfo[]) _propertyCache[cacheKey];
        }

        internal static PropertyInfo GetCachedProperty(this Type type, string propertyName)
        {
            return type.GetCachedProperties().SingleOrDefault(p => p.Name.ToLower() == propertyName.ToLower());
        }
    }
}
