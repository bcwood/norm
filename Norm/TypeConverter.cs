using System;

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
            if (value == DBNull.Value || value == null)
                return null;

            if (targetType == typeof(Int32))
                value = Convert.ToInt32(value);
            else if (targetType == typeof(Decimal))
                value = Convert.ToDecimal(value);
            else if (targetType == typeof(Double))
                value = Convert.ToDouble(value);
            else if (targetType == typeof(Boolean))
                value = Convert.ToBoolean(value);
            else if (targetType == typeof(Char))
                value = Convert.ToChar(value);
            else if (targetType == typeof(Guid))
                value = Guid.Parse(value.ToString());

            return value;
        }
    }
}
