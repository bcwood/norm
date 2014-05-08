using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Norm.QueryBuilder
{
    public class SelectBuilder<T> : BaseQueryBuilder
    {
        private string _top;
        private string _where;
        private List<string> _orderByFields;

        public SelectBuilder() : base(typeof(T))
        {
            _orderByFields = new List<string>();
        }

        public SelectBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
            {
                var expressionVisitor = new SqlExpressionVisitor();
                expressionVisitor.Visit(expression);

                _where = expressionVisitor.ToSqlString();
                base.Parameters = expressionVisitor.Parameters;
            }

            return this;
        }

        public SelectBuilder<T> Top(int count)
        {
            if (count <= 0)
                throw new ArgumentException("TOP must be greater than 0.");

            _top = string.Format("TOP {0} ", count);

            return this;
        }

        public SelectBuilder<T> OrderBy(Expression<Func<T, object>> expression, SortDirection direction = SortDirection.Asc)
        {
            MemberExpression body = expression.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression) expression.Body;
                body = ubody.Operand as MemberExpression;
            }

            _orderByFields.Add(string.Format("[{0}] {1}", body.Member.Name, direction.ToString().ToUpper()));

            return this;
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

    public enum SortDirection
    {
        Asc,
        Desc
    }
}
