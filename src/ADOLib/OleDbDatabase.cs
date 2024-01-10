using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace ADOLib
{
    /// <summary>
    /// Using OleDB data provider.
    /// </summary>
    public abstract class OleDbDatabase : AbstractDatabase {

        protected OleDbDatabase() : base() { }

        public override DbConnection CreateConnection() {
			return new OleDbConnection();
		}

		public override DbCommand CreateCommand() {
			return new OleDbCommand();
		}

		public override DbDataAdapter CreateDataAdapter() {
			return new OleDbDataAdapter();
		}

        public override DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating,
            Action<object, RowUpdatedEventArgs>? updated) {
            var adapter = new OleDbDataAdapter();
            if (updating != null)
                adapter.RowUpdating += (s, e) => updating(s, e);
            if (updated != null)
                adapter.RowUpdated += (s, e) => updated(s, e);
            return adapter;
        }

        public override IDataParameter CreateParameter() {
            return new OleDbParameter();
        }

        public DataView GetTables() {
            var cnn = (OleDbConnection)this.Connection;
            return cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                new object[] { null, null, null, "TABLE" }).DefaultView;
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            return new OleDbCommandBuilder();
        }

        public override void DeriveParameters(IDbCommand cmd)  {
            if (cmd == null || cmd.CommandType != CommandType.StoredProcedure)
                return;

            bool isConnection = false;
            if (cmd.Connection == null)
            {
                cmd.Connection = this.Connection;
                isConnection = true;
            }

            OleDbCommandBuilder.DeriveParameters((OleDbCommand)cmd);

            if (isConnection)
                cmd.Connection = null;

            foreach (OleDbParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput)
                    p.Value = DBNull.Value;
            }
        }
    }
}
