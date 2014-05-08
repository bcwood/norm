using System.Collections.Generic;

namespace Norm.QueryBuilder
{
    internal interface IQueryBuilder
    {
        Dictionary<string, object> Parameters { get; set; }
        string ToSqlString();
    }
}
