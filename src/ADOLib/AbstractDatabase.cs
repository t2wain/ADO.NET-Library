using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data;
using System.Data.Common;

namespace ADOLib
{

    /// <summary>
    /// Provide convenience methods to work with data
    /// using ADO.NET while using only common types from
    /// System.Data.Common which are implemented
    /// by many data providers. Each specific data provider
    /// will inherit this base class and implements those abstract
    /// methods that return types specific to the data provider.
    /// </summary>
    public abstract class AbstractDatabase : IDatabase {

        #region Other

        //private bool _disposeConnection;
        private string _connString = "";
        private DbConnection? _conn = null;
        private IDbTransaction? _tran = null;

        public ILogger Logger { get; set; } = NullLogger.Instance;

        #endregion Other

        #region Connection

        /// <summary>
        /// If the connection string is different than the previous value,
        /// the current cached DB connection will be closed.
        /// </summary>
        public string ConnectionString {
            get {
                if (this._connString == String.Empty) 
                    this._connString = this.GetConnectionString();
                return this._connString;
            }
            set {
                if (this._connString.CompareTo(value) != 0) 
                    this.CleanUp();
                this._connString = value;
                Logger.LogInformation(new EventId(4, "ConnectionString Set"), "A new connection string is set");
            }
        }

        /// <summary>
        /// Allow override to get connection string from
        /// configuration file.
        /// </summary>
        public virtual string GetConnectionString() => "";

        /// <summary>
        /// Return the cache DB connection, or
        /// open a new connection and cache it, or
        /// store an external connection for internal use.
        /// Note, current cache connection, if any, will 
        /// be closed first.
        /// </summary>
        public DbConnection Connection {
            get {
                if (this._conn == null && this.ConnectionString == "")
                    throw (new Exception("Unable to open database connection"));
                if (this._conn == null) {
                    this._conn = this.CreateConnection();
                    this._conn.StateChange += ((o, e) => 
                        Logger.LogInformation("Connection state changed: {0}", _conn.State));
                    this._conn.ConnectionString = this.ConnectionString;
                    Logger.LogInformation(new EventId(0, "Connection Get"), 
                        string.Format("Initialize a new connection : {0}", _conn.State));
                }
                if (this._conn.State == ConnectionState.Closed) {
                    this._conn.Open();
                    if (this._conn.State != ConnectionState.Open)
                        throw (new Exception("Unable to open database connection"));
                    Logger.LogInformation(new EventId(0, "Connection Get"), 
                        string.Format("Open a connection : {0}", _conn.State));
                }
                Logger.LogInformation(new EventId(0, "Connection Get"), 
                    string.Format("Return a connection : {0}", _conn.State));
                return this._conn;
            }
            set {
                if (this._conn != null) this.CleanUp();
                this._conn = value;
                Logger.LogInformation(new EventId(0, "Connection Set"), "An external connection is set as property.");
            }
        }

        /// <summary>
        /// Close the cached DB connection
        /// </summary>
        public void CloseConnection() {
            if (_conn?.State == ConnectionState.Open)
            {
                _conn.Close();
                Logger.LogInformation(new EventId(1, "CloseConnection"), 
                    string.Format("Closed a connection : {0}", _conn.State));
            }
        }

        #endregion Connection

        #region ExecuteDataReader

        /// <summary>
        /// Note, the underlying DB connection will also closed
        /// when the IDataReader is closed.
        /// </summary>
        public DataReaderResult ExecuteDataReader(string sql) {
            using var cmd = CreateCommand();
            cmd.CommandText = ParseSQLParamName(sql);
            var reader = ExecuteDataReader(cmd);
            return reader;
        }

        /// <summary>
        /// Note, the underlying DB connection will also closed
        /// when the IDataReader is closed.
        /// </summary>
        public DataReaderResult ExecuteDataReader(IDbCommand cmd) {
            cmd.Connection ??= this.Connection;
            return new(cmd.ExecuteReader(), this);
        }

        #endregion ExecuteDataReader

        #region Execute

        public int Execute(string sql) {
            using var cmd = CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            return this.Execute(cmd);
        }

        public int Execute(IDbCommand cmd) {
            cmd.Connection ??= this.Connection;
            return cmd.ExecuteNonQuery();
        }

        #endregion Execute

        #region ExecuteDataSet

        /// <summary>
        /// Execute a SELECT sql statement and return
        /// the data result as a table in a dataset.
        /// </summary>
        public DataSet ExecuteDataSet(string sql, string tableName) {
            using var cmd = this.CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            return this.ExecuteDataSet(cmd, tableName);
        }

        /// <summary>
        /// Execute a SELECT command and return
        /// the data result as a table in a dataset.
        /// </summary>
        public DataSet ExecuteDataSet(IDbCommand cmd, string tableName) {
            var dataSet = new DataSet();
            this.ExecuteDataSet(cmd, tableName, dataSet);
            return dataSet;
        }

        /// <summary>
        /// Execute a SELECT sql statement and return
        /// the data result as a table in an existing dataset.
        /// </summary>
        public void ExecuteDataSet(string sql, string tableName, DataSet dataSet) {
            using var cmd = this.CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            this.ExecuteDataSet(cmd, tableName, dataSet);
        }

        /// <summary>
        /// Execute a SELECT command and return
        /// the data result as a table in an existing dataset.
        /// </summary>
        public void ExecuteDataSet(IDbCommand cmd, string tableName, DataSet dataSet) {
            cmd.Connection = this.Connection;
            using var da = this.CreateDataAdapter();
            da.SelectCommand = (DbCommand) cmd;
            object l = dataSet.Tables.Contains(tableName) ? dataSet.Tables[tableName]! : dataSet;
            da.MissingSchemaAction = MissingSchemaAction.Add;
            lock (l) { da.Fill(dataSet, tableName); }
        }

        public virtual DataTable ExecuteProcedureSelect(IDbCommand cmd, string cursorName, string tableName) {
            throw new NotImplementedException();
        }

        #endregion ExecuteDataSet

        #region ExecuteTable

        public DataTable ExecuteTable(string sql, string tableName) {
            using var cmd = this.CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            return this.ExecuteTable(cmd, tableName);
            
        }

        public DataTable ExecuteTable(IDbCommand cmd, string tableName) {
            var table = new DataTable();
            this.ExecuteTable(cmd, table);
            return table;
        }

        public void ExecuteTable(string sql, DataTable table) {
            var cmd = this.CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            this.ExecuteTable(cmd, table);
        }

        public void ExecuteTable(IDbCommand cmd, DataTable table) {
            cmd.Connection = this.Connection;
            using var da = this.CreateDataAdapter();
            da.SelectCommand = (DbCommand)cmd;
            da.MissingSchemaAction = MissingSchemaAction.Add;
            lock (table) { da.Fill(table); }
        }

        #endregion
        
        #region Transaction

        public IDbTransaction BeginTransaction() {
            if (this._tran != null) this.RollbackTransaction();
            this._tran = this.Connection.BeginTransaction();
            return this._tran;
        }

        public void CommitTransaction() {
            if (this._tran != null) {
                this._tran.Commit();
                this._tran = null;
                this.CloseConnection();
            }
        }

        public void RollbackTransaction() {
            if (this._tran != null) {
                this._tran.Rollback();
                this._tran = null;
                this.CloseConnection();
            }
        }

        #endregion Transaction

        #region CleanUp

        public void Dispose() 
        {
            Logger.LogInformation(new EventId(2, "Dispose"), "Disposing the Database instance");
            this.CleanUp();
        }

        protected virtual void CleanUp() {
            try {
                if (this._tran != null) {
                    this._tran.Rollback();
                    this._tran = null;
                    Logger.LogInformation(new EventId(3, "CleanUp"), "Roll back existing transaction");
                }

                if (this._conn != null) {
                    Logger.LogInformation(new EventId(3, "CleanUp"), string.Format("Connection state : {0}", _conn.State));
                    if (this._conn.State == ConnectionState.Open)
                    {
                        this._conn.Close();
                        Logger.LogInformation(new EventId(3, "CleanUp"), 
                            string.Format("Close existing connection : {0}", _conn.State));
                    }
                    this._conn.Dispose();
                    this._conn = null;
                    Logger.LogInformation(new EventId(3, "CleanUp"), "Dispose connection");
                }
            }
            catch { }
        }

        #endregion CleanUp

        #region Parameter

        public IDataParameter CreateParameter(string name, object value) {
            var p = this.CreateParameter();
            p.ParameterName = this.ParseParamName(name);
            p.Value = value;
            return p;
        }

        public IDataParameter CreateOutParameter(string name, DbType type) {
            var p = this.CreateParameter();
            p.ParameterName = this.ParseParamName(name);
            p.Direction = ParameterDirection.Output;
            p.DbType = type;
            return p;
        }

        public virtual string ParseParamName(string paramName) {
            return paramName;
        }

        /// <summary>
        /// The parameter name of some data provider
        /// may contain prefix character like : or @
        /// that need to be remove to get a common name
        /// </summary>
        public virtual string GetCommonParamName(string paramName) {
            return paramName;
        }

        #endregion Parameter

        #region Command

        public DbCommand CreateCommand(string sql) {
            return this.CreateCommand(sql, CommandType.Text);
        }

        public DbCommand CreateCommand(string sql, CommandType type) {
            var cmd = this.CreateCommand();
            cmd.CommandText = this.ParseSQLParamName(sql);
            cmd.CommandType = type;
            return cmd;
        }

        public virtual string ParseSQLParamName(string sql) {
            return sql;
        }

        #endregion Command

        #region Abstract methods

        // implement by sub-class for the specific type of .NET data providers
        // such as OleDb, SqlServer and Oracle
        public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateCommand();
        public abstract DbDataAdapter CreateDataAdapter();
        public abstract DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating,
            Action<object, RowUpdatedEventArgs>? updated);
        public abstract IDataParameter CreateParameter();
        public abstract DbCommandBuilder CreateCommandBuilder();
        public abstract void DeriveParameters(IDbCommand cmd);

        #endregion
    }

}
