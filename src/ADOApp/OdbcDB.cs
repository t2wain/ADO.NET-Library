using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADOLib;

namespace ADOApp
{
    public class OdbcDB : ODBCDatabase, IOdbcDB
    {
        private IConfiguration _config;

        public OdbcDB(IConfiguration config, ILogger<OdbcDB> logger) : base()
        {
            _config = config;
            Logger = logger;
        }

        public override string GetConnectionString() => _config.GetConnectionString("Odbc")!;

    }

    public interface IOdbcDB : IDatabase {}
}
