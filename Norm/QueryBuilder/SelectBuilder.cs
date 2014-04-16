using System;
using System.Linq.Expressions;

namespace Norm.QueryBuilder
{
    public class SelectBuilder<T> : BaseQueryBuilder
    {
        public SelectBuilder() : base(typeof(T))
        {
            base.Append("SELECT * FROM [{0}]", typeof(T).Name);
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
    }
}
