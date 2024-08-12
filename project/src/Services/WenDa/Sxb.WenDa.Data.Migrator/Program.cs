// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Sxb.WenDa.Data.Migrator;
using Sxb.WenDa.Query.ElasticSearch;

Console.WriteLine("---------------------开始运行!---------------------");

//dotnet ef migrations add Inital

//dotnet ef database update
//dotnet ef migrations add xxx
//dotnet ef database update drop

//using var db = new LocalDbContextFactory().CreateDbContext(null);
//var questions = db.Questions.ToList();

var serviceProvider = new Startup().ServiceProvider;

CreateEsIndex(serviceProvider);


Console.WriteLine("---------------------运行成功!---------------------");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();


void CreateEsIndex(IServiceProvider serviceProvider)
{
    var types = typeof(IQuestionEsRepository).Assembly
        .GetTypes()
        .Where(s => s.IsInterface && s.GetInterface("IBaseEsRepository`1") != null)
        .ToArray();

    bool forceDelete = true;
    foreach (var type in types)
    {
        var esRepository = serviceProvider.GetService(type);

        var realType = esRepository.GetType();
        realType.GetMethod("CreateIndex", new[] { typeof(bool) }).Invoke(esRepository, new object[] { forceDelete });

        //var addTestMethod = realType.GetMethod("AddTest");
        //if (addTestMethod != null) addTestMethod.Invoke(esRepository, null);
    }
}