using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace ADOLib
{
    /// <summary>
    /// Using Oracle ODP.NET data providers
    /// </summary>
    public abstract class OracleDatabase : AbstractDatabase {

        protected OracleDatabase() : base() { }

        public override DbConnection CreateConnection() {
			return new OracleConnection();
		}

		public override DbCommand CreateCommand() {
            var cmd = new OracleCommand();
            cmd.BindByName = true;
			return cmd;
		}

		public override DbDataAdapter CreateDataAdapter() {
			return new OracleDataAdapter();
		}

        public override DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating,
            Action<object, RowUpdatedEventArgs>? updated) {
            var adapter = new OracleDataAdapter();
            if (updating != null)
                adapter.RowUpdating += (s, e) => updating(s, e);
            if (updated != null)
                adapter.RowUpdated += (s, e) => updated(s, e);
            return adapter;
        }

        public override IDataParameter CreateParameter() {
            return new OracleParameter();
        }

        public override string ParseParamName(string paramName) {
            return paramName.Replace("@", ":");
        }

        public override string ParseSQLParamName(string sql) {
            return sql.Replace("@", ":");
        }

        public override DataTable ExecuteProcedureSelect(IDbCommand cmd, string cursorName, string tableName) {
            var p = new OracleParameter(cursorName, OracleDbType.RefCursor);
            p.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(p);

            using var dr = this.ExecuteDataReader(cmd).DataReader;
            var t = new DataTable(tableName);
            t.Load(dr!);
            return t;
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            return new OracleCommandBuilder();
        }

        public override void DeriveParameters(IDbCommand cmd) {
            if (cmd == null || cmd.CommandType != CommandType.StoredProcedure)
                return;

            var isConnection = false;
            if (cmd.Connection == null) {
                cmd.Connection = this.Connection;
                isConnection = true;
            }

            OracleCommandBuilder.DeriveParameters((OracleCommand)cmd);

            if (isConnection)
                cmd.Connection = null;

            foreach (OracleParameter p in cmd.Parameters) {
                if (p.OracleDbType == OracleDbType.RefCursor)
                    p.Direction = ParameterDirection.Output;
                else if (p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput)
                    p.Value = DBNull.Value;
            }
        }

        public override string GetCommonParamName(string paramName) {
            return paramName.Replace(":", "");
        }
    }

}
