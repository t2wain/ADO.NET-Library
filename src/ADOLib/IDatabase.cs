using System.Data;
using System.Data.Common;

namespace ADOLib
{
    /// <summary>
    /// Provide convenience methods to work with data
    /// using ADO.NET while using only common types from
    /// System.Data.Common which are implemented
    /// by many data providers.
    /// </summary>
    public interface IDatabase : IDisposable {
        string ConnectionString { get; set; }
        DbConnection Connection { get; set; }
        void CloseConnection();

        DataReaderResult ExecuteDataReader(string sql);
        DataReaderResult ExecuteDataReader(IDbCommand cmd);
        
        int Execute(string sql);
        int Execute(IDbCommand cmd);
        
        DataSet ExecuteDataSet(string sql, string tableName);
        DataSet ExecuteDataSet(IDbCommand cmd, string tableName);
        void ExecuteDataSet(string sql, string tableName, DataSet dataSet);
        void ExecuteDataSet(IDbCommand cmd, string tableName, DataSet dataSet);
        DataTable ExecuteProcedureSelect(IDbCommand cmd, string cursorName, string tableName);

        DataTable ExecuteTable(string sql, string tableName);
        DataTable ExecuteTable(IDbCommand cmd, string tableName);
        void ExecuteTable(string sql, DataTable table);
        void ExecuteTable(IDbCommand cmd, DataTable table);

        IDbTransaction BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        DbCommand CreateCommand();
        DbCommand CreateCommand(string sql);
        DbCommand CreateCommand(string sql, CommandType type);
        string ParseSQLParamName(string sql);

        IDataParameter CreateParameter();
        IDataParameter CreateParameter(string name, object value);
        IDataParameter CreateOutParameter(string name, DbType type);

        string ParseParamName(string paramName);
        void DeriveParameters(IDbCommand cmd);
        string GetCommonParamName(string paramName);

        DbDataAdapter CreateDataAdapter();
        DbDataAdapter CreateDataAdapter(Action<object, RowUpdatingEventArgs>? updating, 
            Action<object, RowUpdatedEventArgs>? updated);
        DbCommandBuilder CreateCommandBuilder();

    }

    public record DataReaderResult(IDataReader DataReader, IDatabase Database) : IDisposable
    {
        public void Dispose()
        {
            this.DataReader.Close();
            this.DataReader.Dispose();
            this.Database.Dispose();
        }
    }

}
