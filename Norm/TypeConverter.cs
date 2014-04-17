using System;
using System.ComponentModel;

namespace Norm
{
    public static class TypeConverter
    {
        /// <summary>
        /// Convert an object to the given target type.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <param name="targetType">Type to convert object to.</param>
        /// <returns>Returns an object converted to type <paramref name="targetType"/></returns>
        public static object ConvertToType(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var typeConverter = TypeDescriptor.GetConverter(targetType);
            return typeConverter.ConvertFromInvariantString(value.ToString());
        }
    }
}
