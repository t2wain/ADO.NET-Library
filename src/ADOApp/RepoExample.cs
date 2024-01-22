using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Data.Common;
using ADOLib;

namespace ADOApp
{
    /// <summary>
    /// An example of a repository using a
    /// non-specific data provider.
    /// </summary>
    public class RepoExample : IDisposable
    {
        private IServiceProvider _provider;

        /// <summary>
        /// This example repository demonstrate the using
        /// of a non-specific data provider.
        /// </summary>
        /// <param name="db">A non-specific database provider</param>
        public RepoExample(IServiceProvider provider)
        {
            this._provider = provider;
        }

        public void OpenClose()
        {
            using var db = NewAccessDB();
            DbConnection cnn = null!;
            foreach (var idx in Enumerable.Range(0, 5))
            {
                // when obtain a connection
                // it needed to be closed
                try { cnn = db.Connection; }
                finally { db.CloseConnection(); }
            }
        }

        public DataTable GetTable()
        {
            using var db = NewAccessDB();
            var sql = "select * from q_portfolio";
            return db.ExecuteTable(sql, "MyTable")!;
        }

        public DataTable GetTable2()
        {
            using var db = NewAccessDB();
            using var cmd = db.CreateCommand();
            var sql = "select * from q_portfolio";
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            return db.ExecuteTable(cmd, "MyTable")!;
        }

        public DataReaderResult GetReader()
        {
            // Normally, we should not return
            // a DataReader outside of a class.
            // Cannot dispose db just yet,
            // DataReader still require connection
            // Remember to close DataReader to also close the Connection.
            var db = NewAccessDB(); 
            var sql = "select * from q_portfolio";
            return db.ExecuteDataReader(sql)!;
        }

        public DataReaderResult GetReader2()
        {
            // Normally, we should not return
            // a DataReader outside of a class.
            // Cannot dispose db just yet,
            // DataReader still requires connection.
            // Remember to close DataReader to also close the Connection.
            var db = NewAccessDB();
            var sql = "select * from q_portfolio";
            using var cmd = db.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            return db.ExecuteDataReader(cmd)!;
        }

        public DataTableAbstract GetAdapter()
        {
            var db = _provider.GetRequiredService<IAccessDB>();
            var t = new DataTableAbstract(db);
            var o = new DataTableAbstract.Options()
            {
                TableName = "q_portfolio",
                DbTableViewName = "q_portfolio"
            };
            t.Init(o);
            t.RefreshData();
            return t;
        }

        public void GetTables()
        {
            using var db = (ExcelDatabase)NewExcelDB();
            var dv = db.GetTables();
        }

        protected IDatabase NewAccessDB() => _provider.GetRequiredService<IAccessDB>();

        protected IDatabase NewExcelDB() => _provider.GetRequiredService<IExcelDB>();

        protected IDatabase NewOleDB() => _provider.GetRequiredService<IOleDbOracle>();

        public void Dispose()
        {
        }

    }
}
