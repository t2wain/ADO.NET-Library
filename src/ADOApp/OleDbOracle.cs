using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADOLib;

namespace ADOApp
{
    public class OleDbOracle : OleDbDatabase, IOleDbOracle
    {
        private IConfiguration _config;

        public OleDbOracle(IConfiguration config, ILogger<OleDbOracle> logger) : base()
        {
            _config = config;
            Logger = logger;
        }

        public override string GetConnectionString() => _config.GetConnectionString("OleDbOracle")!;
    }

    public interface IOleDbOracle : IDatabase { }
}
