using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ADOLib;

namespace ADOApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider Initialize()
        {
            var c = GetConfig();
            IServiceCollection services = new ServiceCollection();
            return services.ConfigureMyApp(c);
        }

        public static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            if (File.Exists("appSettings.Development.json"))
                builder.AddJsonFile("appSettings.Development.json");

            return builder.Build();
        }

        public static IServiceProvider ConfigureMyApp(this IServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<IConfigurationRoot>(config);

            services.AddSingleton<IConfiguration>(config);

            services.AddLogging(configLog =>
            {
                configLog.AddConsole();
            });

            services.AddTransient<IOracleDB, OracleDB>();
            services.AddTransient<IAccessDB, AccessDB>();
            services.AddTransient<IOdbcDB, OdbcDB>();
            services.AddTransient<IExcelDB, ExcelDB>();
            services.AddTransient<IOleDbOracle, OleDbOracle>();

            services.AddTransient<RepoExample>();

            services.AddTransient<Examples>();

            return services.BuildServiceProvider();
        }

    }
}
