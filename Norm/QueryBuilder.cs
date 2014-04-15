using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Norm
{
    public class QueryBuilder<T> : ExpressionVisitor
    {
        public Dictionary<string, object> Parameters { get; private set; }
        
        private StringBuilder _builder;
        private string _currentParamName;

        public QueryBuilder()
        {
            _builder = new StringBuilder();
            this.Parameters = new Dictionary<string, object>();

            this.Append(string.Format("SELECT * FROM [{0}]", typeof(T).Name));
        }

        public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression != null)
            {
                this.Append(" WHERE ");
                this.Visit(expression);
            }

            return this;
        }

        public string ToSqlString()
        {
            return _builder.ToString();
        }

        private void Append(object p_value)
        {
            _builder.Append(p_value);
        }

        private void Append(string format, params object[] args)
        {
            _builder.AppendFormat(format, args);
        }

        #region ExpressionVisitor overrides

        // ExpressionVisitor implementation adapted from the IQueryable Toolkit: http://blogs.msdn.com/b/mattwar/archive/2008/11/18/linq-links.aspx

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null)
            {
                switch (m.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        _currentParamName = m.Member.Name;
                        this.Append("[{0}]", _currentParamName);
                        return m;
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Constant:
                        var lambda = Expression.Lambda(m);
                        dynamic func = lambda.Compile();
                        var value = func();
                        this.VisitValue(value);
                        return m;
                }
            }

            return base.VisitMember(m);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            this.VisitValue(c.Value);
            return c;
        }

        private void VisitValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                this.Append("NULL");
            }
            else
            {
                this.Append("@{0}", _currentParamName);
                this.Parameters.Add(_currentParamName, value);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(string))
            {
                switch (m.Method.Name)
                {
                    case "StartsWith":
                        this.Visit(m.Object);
                        this.Append(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        this.Append(" + '%'");
                        return m;
                    case "EndsWith":
                        this.Visit(m.Object);
                        this.Append(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        return m;
                    case "Contains":
                        this.Visit(m.Object);
                        this.Append(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        this.Append(" + '%'");
                        return m;
                    case "IsNullOrEmpty":
                        this.Append("(");
                        this.Visit(m.Arguments[0]);
                        this.Append(" IS NULL OR ");
                        this.Visit(m.Arguments[0]);
                        this.Append(" = '')");
                        return m;
                }
            }

            throw new NotSupportedException(string.Format("The method {0}.{1} is not supported", m.Method.DeclaringType, m.Method.Name));
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    var c = b.Right as ConstantExpression;
                    if (c != null && c.Value == null)
                        this.Append(" IS ");
                    else
                        this.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    this.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            this.Visit(b.Right);

            return b;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this.Append("NOT (");
                    this.Visit(u.Operand);
                    this.Append(")");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        #endregion // ExpressionVisitor overrides
    }
}
