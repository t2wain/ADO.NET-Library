using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace ADOLib
{
    /// <summary>
    /// Using ODBC data provider.
    /// </summary>
    public abstract class ODBCDatabase : AbstractDatabase {

        protected ODBCDatabase() : base() { }

        public override DbConnection CreateConnection() {
			return new OdbcConnection();
		}

		public override DbCommand CreateCommand() {
			return new OdbcCommand();
		}

		public override DbDataAdapter CreateDataAdapter() {
			return new OdbcDataAdapter();
		}

        public override DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating,
            Action<object, RowUpdatedEventArgs>? updated) {
            var adapter = new OdbcDataAdapter();
            if (updating != null)
                adapter.RowUpdating += (s, e) => updating(s, e);
            if (updated != null)
                adapter.RowUpdated += (s, e) => updated(s, e);
            return adapter;
        }

        public override IDataParameter CreateParameter() {
            return new OdbcParameter();
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            return new OdbcCommandBuilder();
        }

        public override void DeriveParameters(IDbCommand cmd) {
            if (cmd == null || cmd.CommandType != CommandType.StoredProcedure)
                return;

            bool isConnection = false;
            if (cmd.Connection == null)
            {
                cmd.Connection = this.Connection;
                isConnection = true;
            }

            OdbcCommandBuilder.DeriveParameters((OdbcCommand)cmd);

            if (isConnection)
                cmd.Connection = null;

            foreach (OdbcParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput)
                    p.Value = DBNull.Value;
            }
        }
    }
}
