namespace Norm.QueryBuilder
{
    internal class DeleteBuilder : BaseQueryBuilder
    {
        public DeleteBuilder(object obj) : base(obj.GetType())
        {
            object primaryKeyValue = base.PrimaryKey.GetValue(obj, null);

            base.Parameters.Add(base.PrimaryKey.Name, primaryKeyValue);

            base.Append("DELETE FROM [{0}] WHERE [{1}]=@{1}",
                        base.Type.Name, base.PrimaryKey.Name);
        }
    }
}
