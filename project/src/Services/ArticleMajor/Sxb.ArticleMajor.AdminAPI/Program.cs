using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.AdminAPI.Extensions;
using Sxb.ArticleMajor.AdminAPI.Filters;
using Sxb.Framework.AspNetCoreHelper.JsonConvertExtension;
using Sxb.Framework.AspNetCoreHelper.Utils;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMvc(p =>
{
    p.Filters.Add(typeof(AccessFilter));
    p.Filters.Add(new TypeFilterAttribute(typeof(iSchool.Authorization.AuthorizeAttribute))//iSchool.Auth
    {
        Arguments = new object[] { false }
    });
});

builder.Services.AddControllers().AddJsonOptions(p =>
{
    p.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    p.JsonSerializerOptions.Converters.Add(new DateTimeNullConverter());
});

#region iSchool.Auth
string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/shared-auth-ticket-keys/";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "iSchoolConsoleAuth";
    options.Cookie.Domain = ".sxkid.com";
    options.Cookie.Path = "/";
    options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(filePath));
});
#endregion


// Add Database
builder.Services.AddSqlServerDataBase(builder.Configuration.GetSection("ConnectionString"));
// Add Swagger
builder.Services.AddSwaggerDocument(p =>
{
    p.GenerateEnumMappingDescription = true;
});
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(p =>
{
    p.RegisterModule<AutofacExtension>();
});


var app = builder.Build();

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
                    p.ServerUrl = "/articleMajorAdmin";
                    p.DocumentPath = "/articleMajorAdmin/swagger/{documentName}/swagger.json";
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
                            Url = $"/articleMajorAdmin"
                        }) ;
                    };
                });
#endif
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
