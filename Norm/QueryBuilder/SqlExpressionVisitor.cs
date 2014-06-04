using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Norm.QueryBuilder
{
    // ExpressionVisitor implementation adapted from the IQueryable Toolkit: http://blogs.msdn.com/b/mattwar/archive/2008/11/18/linq-links.aspx
    internal class SqlExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<string, object> Parameters { get; private set; }
        
        private StringBuilder _queryBuilder;
        private string _currentParamName;

        public SqlExpressionVisitor()
        {
            this.Parameters = new Dictionary<string, object>();
            _queryBuilder = new StringBuilder();
        }

        public string ToSqlString()
        {
            return _queryBuilder.ToString();
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null)
            {
                switch (m.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        _currentParamName = m.Member.Name;
                        _queryBuilder.AppendFormat("[{0}]", _currentParamName);
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
                _queryBuilder.Append("NULL");
            }
            else
            {
                _queryBuilder.AppendFormat("@{0}", _currentParamName);
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
                        _queryBuilder.Append(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        _queryBuilder.Append(" + '%'");
                        return m;
                    case "EndsWith":
                        this.Visit(m.Object);
                        _queryBuilder.Append(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        return m;
                    case "Contains":
                        this.Visit(m.Object);
                        _queryBuilder.Append(" LIKE '%' + ");
                        this.Visit(m.Arguments[0]);
                        _queryBuilder.Append(" + '%'");
                        return m;
                    case "IsNullOrEmpty":
                        _queryBuilder.Append("(");
                        this.Visit(m.Arguments[0]);
                        _queryBuilder.Append(" IS NULL OR ");
                        this.Visit(m.Arguments[0]);
                        _queryBuilder.Append(" = '')");
                        return m;
					case "ToUpper":
						_queryBuilder.Append("UPPER(");
						this.Visit(m.Object);
						_queryBuilder.Append(")");
						return m;
					case "ToLower":
						_queryBuilder.Append("LOWER(");
						this.Visit(m.Object);
						_queryBuilder.Append(")");
						return m;
					case "Trim":
						_queryBuilder.Append("LTRIM(RTRIM(");
						this.Visit(m.Object);
						_queryBuilder.Append("))");
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
                    _queryBuilder.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _queryBuilder.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    var c = b.Right as ConstantExpression;
                    if (c != null && c.Value == null)
                        _queryBuilder.Append(" IS ");
                    else
                        _queryBuilder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _queryBuilder.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    _queryBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _queryBuilder.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    _queryBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _queryBuilder.Append(" >= ");
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
                    _queryBuilder.Append("NOT (");
                    this.Visit(u.Operand);
                    _queryBuilder.Append(")");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }
    }
}
