using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace ADOLib
{
    /// <summary>
    /// Using SqlServer client.
    /// </summary>
    public abstract class SqlServerDatabase : AbstractDatabase {

        protected SqlServerDatabase() : base() { }

        public override DbConnection CreateConnection() {
			return new SqlConnection();
		}

		public override DbCommand CreateCommand() {
			return new SqlCommand();
		}

		public override DbDataAdapter CreateDataAdapter() {
			return new SqlDataAdapter();
		}

        public override DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating,
            Action<object, RowUpdatedEventArgs>? updated) {
            var adapter = new SqlDataAdapter();
            if (updating != null)
                adapter.RowUpdating += (s, e) => updating(s, e);
            if (updated != null)
                adapter.RowUpdated += (s, e) => updated(s, e);
            return adapter;
        }

        public override IDataParameter CreateParameter() {
            return new SqlParameter();
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            return new SqlCommandBuilder();
        }

        public override void DeriveParameters(IDbCommand cmd) {
            if (cmd == null || cmd.CommandType != CommandType.StoredProcedure)
                return;

            bool isConnection = false;
            if (cmd.Connection == null) {
                cmd.Connection = this.Connection;
                isConnection = true;
            }

            SqlCommandBuilder.DeriveParameters((SqlCommand)cmd);

            if (isConnection)
                cmd.Connection = null;

            foreach (SqlParameter p in cmd.Parameters) {
                if (p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput)
                    p.Value = DBNull.Value;
            }
        }

        public override string GetCommonParamName(string paramName) {
            return paramName.Replace("@", "");
        }
    }

}
