using System;
using System.Collections.Generic;
using System.Reflection;

namespace Norm.QueryBuilder
{
    internal class UpdateBuilder : BaseQueryBuilder
    {
        public UpdateBuilder(object obj) : base(obj.GetType())
        {
            base.ExcludeCreateFields();

            var setList = new List<string>();

            foreach (PropertyInfo property in base.Type.GetCachedProperties())
            {
                if (base.IsExcludedField(property.Name))
                    continue;

                string paramName = property.Name;
                object paramValue = null;

                if (!base.IsPrimaryKey(paramName))
                    setList.Add(string.Format("[{0}]=@{0}", paramName));

                if (base.IsUpdateDateField(paramName))
                {
                    // set updated field
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

            base.Append("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}",
                        base.Type.Name, string.Join(",", setList), base.PrimaryKey.Name);
        }
    }
}
