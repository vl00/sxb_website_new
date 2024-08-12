using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sxb.WenDa.API.Extensions;
using Sxb.Framework.AspNetCoreHelper.JsonConvertExtension;
using Sxb.GenerateNo;
using Sxb.Framework.AspNetCoreHelper.Extensions;
using Sxb.WenDa.Common.Data;
using Sxb.WenDa.API.Config;
using Sxb.Framework.AspNetCoreHelper.Middleware;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Sxb.WenDa.Query.ElasticSearch;
using Sxb.WenDa.Common;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews()
    //.AddJsonOptions(options =>
    //{
    //    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    //    options.JsonSerializerOptions.Converters.Add(new DateTimeNullConverter());
    //    //options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    //})
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //防止引用的包没有驼峰
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        options.SerializerSettings.Converters.Add(new DateTimeToTimestampJsonConverter());
    })
;

builder.Services
    .AddSerilog(Configuration)
    .AddHttpContextAccessor()
    .AddSingleton<ISxbGenerateNo, SxbGenerateNo>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddRedis(Configuration)
    .AddMediatRServices()
    .AddMsSqlDomainContext(Configuration.GetSection("ConnectionString").GetValue<string>("Master"))
    .AddSqlServerDataBase(Configuration.GetSection("ConnectionString"))
    .AddSwaggerDocument(setting => setting.UseControllerSummaryAsTagDescription = true)
    .AddEventBus(Configuration)
    .AddSingleton(Configuration.Get<AppSettingsData>())
    .AddElasticSearch(Configuration)
    .AddHttpClientBase(Configuration.GetSection("ExternalInterface"))
;

#region 登陆状态相关 (需增加 app.UseAuthentication();)
string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
    };
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "iSchoolAuth";
    options.Cookie.Domain = ".sxkid.com";
    options.Cookie.Path = "/";
    options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.ForwardedHeaders = ForwardedHeaders.All;
});
#endregion

builder.Services.Configure<TencentCloudOptions>(Configuration.GetSection("TencentCloudOptions"));
//config autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule<AutofacExtension>();
});


var app = builder.Build();

if (Configuration.GetValue("USE_Forwarded_Headers", false))
{
    app.UseForwardedHeaders();
}
app.AddHttpContextModel();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
#if DEBUG
    app.UseSwaggerUi3(p =>
    {
        p.DocumentPath = "/swagger/{documentName}/swagger.json";//手动设置一下默认,开启explore功能。
    });
    app.UseOpenApi();
#else
                app.UseSwaggerUi3(p =>
                {
                    p.ServerUrl = "/wenda";
                    p.DocumentPath = "/wenda/swagger/{documentName}/swagger.json";
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
                            Url = $"/wenda"
                        }) ;
                    };
                });
#endif

    app.UseStatusCodeMiddleware();
    app.UseExceptionMiddleware();
}
else
{
    app.UseStatusCodeMiddleware();
    app.UseExceptionMiddleware();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Article}/{action=Index}/{id?}");

app.Run();
