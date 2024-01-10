using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADOLib;

namespace ADOApp
{
    /// <summary>
    /// Implement a project specific database connection.
    /// </summary>
    public class OracleDB : OracleDatabase, IOracleDB
    {
        private IConfiguration _config;

        public OracleDB(IConfiguration config, ILogger<OracleDB> logger) : base()
        {
            _config = config;
            Logger = logger;
        }

        public override string GetConnectionString() => _config.GetConnectionString("Oracle")!;
    }

    public interface IOracleDB : IDatabase { }
}
