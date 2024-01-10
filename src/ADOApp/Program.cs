// See https://aka.ms/new-console-template for more information
using ADOApp;
using Microsoft.Extensions.DependencyInjection;
using CFG = ADOApp.ServiceCollectionExtensions;

var provider = CFG.Initialize();

using var ex = provider.GetRequiredService<Examples>();
ex.Run();

GC.Collect();