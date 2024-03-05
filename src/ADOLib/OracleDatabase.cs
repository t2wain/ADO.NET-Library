using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace ADOLib
{
    public interface IOracleDatabase
    {
        IDataParameter CreateParameter(string name, OracleDbType type);
        IDataParameter CreateOutParameter(string name, OracleDbType type);
        IDataParameter CreateReturnParameter(string name, OracleDbType type);
    }

    /// <summary>
    /// Using Oracle ODP.NET data providers
    /// </summary>
    public abstract class OracleDatabase : AbstractDatabase, IOracleDatabase {

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

        public override IDataParameter CreateRefCursorParameter(string name)
        {
            var p = new OracleParameter();
            p.ParameterName = this.ParseParamName(name);
            p.Direction = ParameterDirection.Output;
            p.OracleDbType = OracleDbType.RefCursor;
            return p;
        }

        public override string ParseParamName(string paramName) {
            return paramName.Replace("@", ":");
        }

        public override string ParseSQLParamName(string sql) {
            return sql.Replace("@", ":");
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

        #region IOracleDatabase

        public IDataParameter CreateParameter(string name, OracleDbType type)
        {
            var p = new OracleParameter();
            p.ParameterName = this.ParseParamName(name);
            p.OracleDbType = type;
            return p;
        }

        public IDataParameter CreateOutParameter(string name, OracleDbType type)
        {
            var p = new OracleParameter();
            p.ParameterName = this.ParseParamName(name);
            p.Direction = ParameterDirection.Output;
            p.OracleDbType = type;
            return p;
        }

        public IDataParameter CreateReturnParameter(string name, OracleDbType type)
        {
            var p = new OracleParameter();
            p.ParameterName = this.ParseParamName(name);
            p.Direction = ParameterDirection.ReturnValue;
            p.OracleDbType = type;
            return p;
        }

        #endregion
    }

}
