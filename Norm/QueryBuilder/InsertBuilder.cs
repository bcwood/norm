using System;
using System.Collections.Generic;
using System.Reflection;

namespace Norm.QueryBuilder
{
    internal class InsertBuilder : BaseQueryBuilder
    {
        public InsertBuilder(object obj) : base(obj.GetType())
        {
            base.ExcludeField(base.PrimaryKey.Name);
            base.ExcludeUpdateFields();

            var fieldList = new List<string>();
            var valueList = new List<string>();
            
            foreach (PropertyInfo property in base.Type.GetCachedProperties())
            {
                if (base.IsExcludedField(property.Name))
                    continue;

                string paramName = property.Name;
                object paramValue = null;

                fieldList.Add("[" + paramName + "]");
                valueList.Add("@" + paramName);

                if (base.IsCreateDateField(paramName))
                {
                    paramValue = DateTime.Now;
                    property.SetValue(obj, paramValue, null);
                }
                else
                {
                    paramValue = property.GetValue(obj, null);

                    if (paramValue == null || string.IsNullOrEmpty(paramValue.ToString()))
                        paramValue = DBNull.Value;
                }

                base.Parameters.Add(paramName, paramValue);
            }

            base.Append("INSERT INTO [{0}] ({1})", base.Type.Name, string.Join(",", fieldList));
            base.Append(" VALUES ({0});", string.Join(",", valueList));

            // TODO: handle return of pk based on db provider type
            base.Append(" SELECT SCOPE_IDENTITY();");
        }
    }
}
