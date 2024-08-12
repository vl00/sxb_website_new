using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sxb.ArticleMajor.API.Extensions;
using Sxb.ArticleMajor.API.Utils;
using Sxb.Framework.AspNetCoreHelper.JsonConvertExtension;
using Sxb.GenerateNo;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new DateTimeNullConverter());
    //options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services
    .AddHttpContextAccessor()
    .AddScoped<RazorViewToStringRenderer>()
    .AddSingleton<ISxbGenerateNo, SxbGenerateNo>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddMongodb(Configuration.GetSection("MongodbConfig"))
    .AddRedis(Configuration)
    .AddSqlServerDataBase(Configuration.GetSection("ConnectionString"))
    .AddSwaggerDocument()
    .AddEventBus(Configuration)
    .AddSingleton(Configuration.Get<AppSettingsData>())
;

//config autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule<AutofacExtension>();
});


var app = builder.Build();

HttpContextHelper.ApplicationBuilder = app;
//init Constant
Constant.Init(app.Services.GetService<AppSettingsData>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else {
#if DEBUG
    app.UseSwaggerUi3(p =>
    {
        p.DocumentPath = "/swagger/{documentName}/swagger.json";//手动设置一下默认,开启explore功能。
    });
    app.UseOpenApi();
#else
                app.UseSwaggerUi3(p =>
                {
                    p.ServerUrl = "/articlemajor";
                    p.DocumentPath = "/articlemajor/swagger/{documentName}/swagger.json";
                });
                app.UseOpenApi(p =>
                {
                    p.PostProcess = (d, r) =>
                    {
                        var documentBaseUrl = d.BaseUrl;
                        d.Servers.Clear();
                        d.Servers.Add(new NSwag.OpenApiServer()
                        {
                            Description = "Lonlykids",
                            Url = $"/articlemajor"
                        }) ;
                    };
                });
#endif
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Article}/{action=Index}/{id?}");

app.Run();
