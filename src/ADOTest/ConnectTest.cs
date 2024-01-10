using System.Data;

namespace ADOTest
{
    public class ConnectTest : IClassFixture<Context>
    {
        private Context _ctx;

        public ConnectTest(Context ctx)
        {
            _ctx = ctx;
        }

        #region Oracle

        [Fact]
        public void Should_connect_Oracle()
        {
            using var db = _ctx.GetOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Oracle2()
        {
            using var db = _ctx.GetOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Oracle3()
        {
            using var db = _ctx.GetOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        #endregion

        #region MSAccess

        [Fact]
        public void Should_connect_MSAccess()
        {
            using var db = _ctx.GetAccessDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_MSAccess2()
        {
            using var db = _ctx.GetAccessDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_MSAccess3()
        {
            using var db = _ctx.GetAccessDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        #endregion

        #region ODBC

        [Fact]
        public void Should_connect_Odbc()
        {
            using var db = _ctx.GetOdbcDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Odbc2()
        {
            using var db = _ctx.GetOdbcDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Odbc3()
        {
            using var db = _ctx.GetOdbcDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        #endregion

        #region Excel

        [Fact]
        public void Should_connect_Excel()
        {
            using var db = _ctx.GetExcelDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Excel2()
        {
            using var db = _ctx.GetExcelDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_Excel3()
        {
            using var db = _ctx.GetExcelDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        #endregion

        #region OleDB

        [Fact]
        public void Should_connect_OleDb()
        {
            using var db = _ctx.GetOleDbOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_OleDb2()
        {
            using var db = _ctx.GetOleDbOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        [Fact]
        public void Should_connect_OleDb3()
        {
            using var db = _ctx.GetOleDbOracleDB();
            Assert.Equal(ConnectionState.Open, db.Connection.State);
        }

        #endregion

    }
}