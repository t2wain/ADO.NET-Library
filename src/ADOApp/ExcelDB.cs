using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ADOLib;

namespace ADOApp
{
    public class ExcelDB : ExcelDatabase, IExcelDB
    {
        private IConfiguration _config;

        public ExcelDB(IConfiguration config, ILogger<ExcelDB> logger) : base()
        {
            _config = config;
            Logger = logger;
        }

        public override string GetConnectionString() => _config.GetConnectionString("Excel")!;

    }

    public interface IExcelDB : IDatabase { }
}
