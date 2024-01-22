using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace ADOLib
{
    public class SqliteDatabase : AbstractDatabase
    {
        public override DbConnection CreateConnection() => SQLiteFactory.Instance.CreateConnection();

        public override DbCommand CreateCommand() => SQLiteFactory.Instance.CreateCommand();

        public override DbDataAdapter CreateDataAdapter() => SQLiteFactory.Instance.CreateDataAdapter();

        public override DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating, Action<object, RowUpdatedEventArgs>? updated)
        {
            var adapter = (SQLiteDataAdapter)this.CreateDataAdapter();
            if (updating != null)
                adapter.RowUpdating += (s, e) => updating(s, e);
            if (updated != null)
                adapter.RowUpdated += (s, e) => updated(s, e);
            return adapter;
        }

        public override IDataParameter CreateParameter() => SQLiteFactory.Instance.CreateParameter();

        public override DbCommandBuilder CreateCommandBuilder() => SQLiteFactory.Instance.CreateCommandBuilder();

        public override void DeriveParameters(IDbCommand cmd) => throw new NotImplementedException();

        public override string GetCommonParamName(string paramName)
        {
            return paramName.Replace(":", "");
        }

    }
}
