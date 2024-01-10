using ADOApp;
using Microsoft.Extensions.DependencyInjection;
using ADOLib;
using CFG = ADOApp.ServiceCollectionExtensions;

namespace ADOTest
{
    public class Context : IDisposable
    {
        IServiceProvider _provider = null!;

        public Context()
        {
            _provider = CFG.Initialize();
        }

        public IDatabase GetOracleDB() => 
            _provider.GetRequiredService<IOracleDB>();

        public IDatabase GetAccessDB() =>
            _provider.GetRequiredService<IAccessDB>();

        public IDatabase GetOdbcDB() =>
            _provider.GetRequiredService<IOdbcDB>();

        public IDatabase GetExcelDB() =>
            _provider.GetRequiredService<IExcelDB>();

        public IDatabase GetOleDbOracleDB() =>
            _provider.GetRequiredService<IOleDbOracle>();


        public void Dispose() 
        {
            _provider = null!;
            GC.Collect();
        }
    }
}
