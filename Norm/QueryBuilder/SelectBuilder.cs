using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Norm.QueryBuilder
{
    internal class SelectBuilder<T> : BaseQueryBuilder where T : new()
    {
        private string _top;
        private string _where;
        private List<string> _orderByFields;

        public SelectBuilder(Expression<Func<T, bool>> where = null)
            : base(typeof(T))
        {
            _orderByFields = new List<string>();

            if (where != null)
            {
                var expressionVisitor = new SqlExpressionVisitor();
                expressionVisitor.Visit(where);

                _where = expressionVisitor.ToSqlString();
                base.Parameters = expressionVisitor.Parameters;
            }
        }

        public SelectBuilder<T> Limit(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Limit must be greater than 0.");

            _top = string.Format("TOP {0} ", count);

            return this;
        }

        public SelectBuilder<T> OrderBy(Expression<Func<T, object>> orderBy)
        {
            _orderByFields.Add(string.Format("[{0}]", GetOrderByFieldName(orderBy)));

            return this;
        }

        public SelectBuilder<T> OrderByDesc(Expression<Func<T, object>> orderBy)
        {
            _orderByFields.Add(string.Format("[{0}] DESC", GetOrderByFieldName(orderBy)));

            return this;
        }

        private string GetOrderByFieldName(Expression<Func<T, object>> orderBy)
        {
            MemberExpression body = orderBy.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)orderBy.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public override string ToSqlString()
        {
            base.Append("SELECT {0}*", _top);
            base.Append(" FROM [{0}]", typeof(T).Name);

            if (!string.IsNullOrWhiteSpace(_where))
                base.Append(" WHERE {0}", _where);

            if (_orderByFields.Count > 0)
                base.Append(" ORDER BY {0}", string.Join(",", _orderByFields));

            return base.ToSqlString();
        }
    }
}
