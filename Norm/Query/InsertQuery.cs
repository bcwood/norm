using System.Data;
using Norm.QueryBuilder;

namespace Norm.Query
{
    internal class InsertQuery : BaseQuery
    {
        public InsertQuery(IDbConnection connection, object obj) : base (connection)
        {
            base.QueryBuilder = new InsertBuilder(obj);
        }
    }
}
