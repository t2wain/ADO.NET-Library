using System.Data;
using System.Data.Common;

namespace ADOLib
{

    /// <summary>
    /// This class will build a DataAdapter based on
    /// the Options provided.
    /// </summary>
    public class DataTableAbstract : IDisposable {

        /// <summary>
        /// Options to setup the DataTableAdapter
        /// </summary>
        public record Options
        {
            /// <summary>
            /// Perform setup for inserting/deleting of data 
            /// </summary>
            public bool AllowAddAndDelete { get; set; }
            /// <summary>
            /// Perform setup for updating existing record
            /// </summary>
            public bool AllowUpdate { get; set; }
            /// <summary>
            /// Event handler for updating action
            /// </summary>
            public Action<object, RowUpdatingEventArgs>? UpdatingAction { get; set; }
            /// <summary>
            /// Event handler for updated action
            /// </summary>
            public Action<object, RowUpdatedEventArgs>? UpdatedAction { get; set; }
            /// <summary>
            /// Parameters for the where clause when getting data
            /// </summary>
            public Dictionary<string, object>? SelectParameters { get; set; } = new();
            /// <summary>
            /// The name of return DataTable
            /// </summary>
            public string? TableName { get; set; }
            /// <summary>
            /// Return data from this table or view name, ex. select * from [DbTableViewName]
            /// </summary>
            public string? DbTableViewName { get; set; }
            /// <summary>
            /// Return an empty recordset from this store procedure
            /// for the purpose of setting up the DataTable
            /// </summary>
            public string? DbSchemaProcName { get; set; }
            /// <summary>
            /// Return data from this stored procedure
            /// </summary>
            public string? DbSelectProcName { get; set; }
            /// <summary>
            /// Insert new record using this stored procedure
            /// </summary>
            public string? DbInsertProcName { get; set; }
            /// <summary>
            /// Update existing record using this stored procedure
            /// </summary>
            public string? DbUpdateProcName { get; set; }
            /// <summary>
            /// Delete existing record using this stored procedure
            /// </summary>
            public string? DbDeleteProcName { get; set; }
        }

        Options _o = null!;
        IDatabase _db = null!;

        public DataTableAbstract(IDatabase db)
        {
            _db = db;
        }

        public DataTable DataTable { get; protected set; } = null!;

        protected DbDataAdapter TableAdapter { get; set; } = null!;

        #region Initialize

        public void Init(Options options, DataTable? tbl = null) {
            if (TableAdapter != null)
            {
                TableAdapter.Dispose();
                TableAdapter = null!;
            }

            _o = options;
            DataTable = tbl ?? new DataTable(_o.TableName);
            TableAdapter = _db.CreateDataAdapter(_o.UpdatingAction, _o.UpdatedAction);
            this.SetupTableSchema();
            this.SetupTableAdapter();
        }

        protected void SetupTableSchema() {
            using var cmd = _db.CreateCommand();

            // setup select command based on options
            if (!string.IsNullOrWhiteSpace(_o.DbSchemaProcName)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = _o.DbSchemaProcName;
            }
            else if (!string.IsNullOrWhiteSpace(_o.DbSelectProcName)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = _o.DbSelectProcName;
            }
            else if (!string.IsNullOrWhiteSpace(_o.DbTableViewName)) {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("select * from {0}", _o.DbTableViewName);
            }

            if (!string.IsNullOrWhiteSpace(cmd.CommandText)) {
                var da = TableAdapter;
                da.SelectCommand = cmd;
                _db.DeriveParameters(cmd);
                if (cmd.Parameters.Count > 0 && _o.SelectParameters != null)
                    this.SetParameterValues(cmd.Parameters, _o.SelectParameters);
                // call DB to get table schema
                cmd.Connection = _db.Connection;
                da.FillSchema(this.DataTable, SchemaType.Source); 
            }
        }

        protected void SetupTableAdapter() {
            var b = _db.CreateCommandBuilder();
            b.DataAdapter = this.TableAdapter;
            b.ConflictOption = ConflictOption.OverwriteChanges;

            DbCommand cmd = null!;
            bool isTableBase = string.IsNullOrWhiteSpace(_o.DbSelectProcName);

            // setup select command
            if (!string.IsNullOrWhiteSpace(_o.DbSelectProcName))
                cmd = this.SetupProcCommand(_o.DbSelectProcName); // using stored procedure
            else if (!string.IsNullOrWhiteSpace(_o.DbTableViewName)) {
                cmd = _db.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = String.Format("select * from {0}", _o.DbTableViewName); // using table/view name
            }
            this.TableAdapter.SelectCommand = cmd;

            // setup update command
            if (_o.AllowUpdate) {
                cmd = null!;
                if (!string.IsNullOrWhiteSpace(_o.DbUpdateProcName))
                    // relate params to column
                    cmd = this.SetupProcCommand(_o.DbUpdateProcName); 
                else if (isTableBase) 
                    cmd = b.GetUpdateCommand(true);
                this.TableAdapter.UpdateCommand = cmd;
            }

            if (_o.AllowAddAndDelete) {
                // setup add command
                cmd = null!;
                if (!string.IsNullOrWhiteSpace(_o.DbInsertProcName))
                    // relate params to column
                    cmd = this.SetupProcCommand(_o.DbInsertProcName); 
                else if (isTableBase) cmd = b.GetInsertCommand(true);
                this.TableAdapter.InsertCommand = cmd;

                // setup update command
                cmd = null!;
                if (!string.IsNullOrWhiteSpace(_o.DbDeleteProcName))
                    // relate params to column
                    cmd = this.SetupProcCommand(_o.DbDeleteProcName);
                else if (isTableBase) cmd = b.GetDeleteCommand(true);
                this.TableAdapter.DeleteCommand = cmd;
            }
        }

        /// <summary>
        /// Setup the Command with stored procedure parameters 
        /// that are related to the corresponding DataTable columns
        /// </summary>
        /// <param name="sqlCommand">SQL stored procdure name</param>
        protected DbCommand SetupProcCommand(string sqlCommand) {
            // query the data to return a set of parameter for this stored procedure
            var cmd = _db.CreateCommand(sqlCommand, CommandType.StoredProcedure);
            _db.DeriveParameters(cmd);

            // get a list of column name for the DataTable
            List<string> colNames = new List<string>();
            foreach (DataColumn c in this.DataTable.Columns)
                colNames.Add(c.ColumnName);

            // setup the parameters for each matching name
            // of DataTable column and stored procedure parameter
            foreach (DbParameter p in cmd.Parameters) {
                string paramName = _db.GetCommonParamName(p.ParameterName);
                if (colNames.Contains(paramName)
                    && (p.Direction == ParameterDirection.Input
                    || p.Direction == ParameterDirection.InputOutput))
                    p.SourceColumn = paramName; // relate the parameter to the column
            }
            return cmd;
        }

        #endregion

        #region Set/Remove connections to command

        protected void SetConnection() {
            UpdateConnection(_db.Connection);
        }

        protected void ClearConnection() {
            UpdateConnection(null);
            _db.CloseConnection();
        }

        protected void UpdateConnection(DbConnection? cnn)
        {
            var ta = TableAdapter;

            if (ta.SelectCommand is var sc && sc != null)
                sc.Connection = cnn;

            if (ta.UpdateCommand is var uc && uc != null)
                uc.Connection = cnn;

            if (ta.InsertCommand is var ic && ic != null)
                ic.Connection = cnn;

            if (ta.DeleteCommand is var dc && dc != null)
                dc.Connection = cnn;
        }

        #endregion

        #region Update data

        public void RefreshData() {
            this.RefreshData(_o.SelectParameters);
        }

        public void RefreshData(IDictionary<string, object> paramValues) {
            DataTable.Clear();
            var cmd = TableAdapter.SelectCommand;
            ClearParams(cmd!.Parameters);
            if (paramValues != null)
                SetParameterValues(cmd.Parameters, paramValues);

            cmd.Connection = _db.Connection;
            TableAdapter.Fill(DataTable);
            cmd.Connection = null;
        }

        protected void ClearParams(DbParameterCollection parameters) {
            foreach (DbParameter p in parameters)
                if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input)
                    p.Value = DBNull.Value;
        }

        protected void SetParameterValues(DbParameterCollection parameters, IDictionary<string, object> paramValues) {
            foreach (DbParameter p in parameters) {
                string paramName = _db.GetCommonParamName(p.ParameterName);
                if ((p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput)
                    && paramValues.ContainsKey(paramName))
                    p.Value = paramValues[paramName];
            }

        }

        public void UpdateData() {
            this.SetConnection();
            this.TableAdapter.Update(this.DataTable);
            this.ClearConnection();
        }

        #endregion

        public void Dispose()
        {
            TableAdapter.Dispose();
            TableAdapter = null!;
            _db.Dispose();
            _db = null!;
        }
    }
}
