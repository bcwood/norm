using System.Data;
using Norm.QueryBuilder;

namespace Norm.Query
{
    internal class UpdateQuery : BaseQuery
    {
        public UpdateQuery(IDbConnection connection, object obj) : base (connection)
        {
            base.QueryBuilder = new UpdateBuilder(obj);
        }
    }
}
