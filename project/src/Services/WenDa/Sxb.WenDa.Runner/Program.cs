// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Sxb.WenDa.Runner;
using Sxb.WenDa.Runner.ImportFromExcel;

Console.WriteLine("Hello, World!");

var serviceProvider = new Startup().ServiceProvider;




serviceProvider.GetService<ImportFromExcelRunner>().Run();


Console.WriteLine("exit!");