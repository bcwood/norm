using System.Data;
using Norm.QueryBuilder;

namespace Norm.Query
{
    internal class DeleteQuery : BaseQuery
    {
        public DeleteQuery(IDbConnection connection, object obj) : base (connection)
        {
            base.QueryBuilder = new DeleteBuilder(obj);
        }
    }
}
