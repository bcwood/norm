using System.Data;
using Norm.QueryBuilder;

namespace Norm.Query
{
    public abstract class BaseQuery
    {
        private IDbConnection Connection { get; set; }
        internal IQueryBuilder QueryBuilder { get; set; }
        
        public BaseQuery(IDbConnection connection)
        {
            this.Connection = connection;
        }

        public object ExecuteScalar()
        {
            IDbCommand command = this.PrepareCommand();
            return command.ExecuteScalar();
        }

        public int ExecuteNonQuery()
        {
            IDbCommand command = this.PrepareCommand();
            return command.ExecuteNonQuery();
        }

        protected IDataReader ExecuteReader()
        {
            IDbCommand command = this.PrepareCommand();
            return command.ExecuteReader();
        }

        private IDbCommand PrepareCommand()
        {
            IDbCommand command = this.Connection.CreateCommand();
            command.CommandText = this.QueryBuilder.ToSqlString();

            // load parameters
            foreach (string key in this.QueryBuilder.Parameters.Keys)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = key;
                parameter.Value = this.QueryBuilder.Parameters[key];

                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}
