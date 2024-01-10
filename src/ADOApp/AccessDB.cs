using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADOLib;

namespace ADOApp
{
    /// <summary>
    /// Implement a project specific database connection.
    /// </summary>
    public class AccessDB : OleDbDatabase, IAccessDB
    {
        private IConfiguration _config;

        public AccessDB(IConfiguration config, ILogger<AccessDB> logger)
        {
            _config = config;
            Logger = logger;
        }

        public override string GetConnectionString() => _config.GetConnectionString("MSAccess")!;

    }

    public interface IAccessDB : IDatabase { }
}
