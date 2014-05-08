using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Norm.QueryBuilder
{
    internal abstract class BaseQueryBuilder : IQueryBuilder
    {
        public Dictionary<string, object> Parameters { get; set; }

        protected Type Type { get; private set; }
        protected PropertyInfo PrimaryKey { get; private set; }

        private StringBuilder _queryBuilder;
        private List<string> _excludedFields;

        private readonly List<string> _createFieldNames = new List<string> { "created", "createdate", "createdon" };
        private readonly List<string> _updateFieldNames = new List<string> { "updated", "updatedate", "updatedon" };
        
        protected BaseQueryBuilder(Type type)
        {
            this.Type = type;
            this.PrimaryKey = type.GetPrimaryKey();
            this.Parameters = new Dictionary<string, object>();

            _queryBuilder = new StringBuilder();
            _excludedFields = new List<string>();
        }

        public virtual string ToSqlString()
        {
            return _queryBuilder.ToString();
        }

        protected void Append(string format, params object[] args)
        {
            _queryBuilder.AppendFormat(format, args);
        }

        protected bool IsPrimaryKey(string fieldName)
        {
            return this.PrimaryKey.Name.ToLower() == fieldName.ToLower();
        }

        protected bool IsExcludedField(string fieldName)
        {
            return _excludedFields.Contains(fieldName.ToLower());
        }

        protected void ExcludeField(string fieldName)
        {
            _excludedFields.Add(fieldName.ToLower());
        }

        protected void ExcludeCreateFields()
        {
            _excludedFields.AddRange(_createFieldNames);
        }

        protected void ExcludeUpdateFields()
        {
            _excludedFields.AddRange(_updateFieldNames);
        }

        protected bool IsCreateDateField(string fieldName)
        {
            return _createFieldNames.Contains(fieldName.ToLower());
        }

        protected bool IsUpdateDateField(string fieldName)
        {
            return _updateFieldNames.Contains(fieldName.ToLower());
        }
    }
}
