// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Sxb.ArticleMajor.Runner;
using Sxb.ArticleMajor.Runner.DynamicImage;
using Sxb.ArticleMajor.Runner.HotData;
using Sxb.ArticleMajor.Runner.ImportAd;
using Sxb.ArticleMajor.Runner.WashContent;

Console.WriteLine("Hello, World!");




var serviceProvider = new Startup().ServiceProvider;



//var dynamicImageRunner = serviceProvider.GetService<DynamicImageRunner>();
//dynamicImageRunner.Run();

//serviceProvider.GetService<ImportAdRunner>().Run();

//serviceProvider.GetService<RemoveDuplicationRunner>().Run();

serviceProvider.GetService<WashContentRunner>().Run();

//serviceProvider.GetService<HotDataRunner>().Run();

Console.WriteLine("exit!");