using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Norm.QueryBuilder
{
    public class SelectBuilder<T> : BaseQueryBuilder
    {
        private List<string> _orderByFields;

        public SelectBuilder() : base(typeof(T))
        {
            base.Append("SELECT * FROM [{0}]", typeof(T).Name);

            _orderByFields = new List<string>();
        }

        public SelectBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
            {
                base.Append(" WHERE ");

                var expressionVisitor = new SqlExpressionVisitor();
                expressionVisitor.Visit(expression);

                base.Append(expressionVisitor.ToSqlString());
                base.Parameters = expressionVisitor.Parameters;
            }

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
