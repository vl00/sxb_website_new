#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/WenDa/Sxb.WenDa.API/Sxb.WenDa.API.csproj", "src/Services/WenDa/Sxb.WenDa.API/"]
COPY ["src/Services/WenDa/Sxb.WenDa.Query.ElasticSearch/Sxb.WenDa.Query.ElasticSearch.csproj", "src/Services/WenDa/Sxb.WenDa.Query.ElasticSearch/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Services/WenDa/Sxb.WenDa.Domain/Sxb.WenDa.Domain.csproj", "src/Services/WenDa/Sxb.WenDa.Domain/"]
COPY ["src/Shared/Sxb.Domain.Abstractions/Sxb.Domain.Abstractions.csproj", "src/Shared/Sxb.Domain.Abstractions/"]
COPY ["src/Services/WenDa/Sxb.WenDa.Common/Sxb.WenDa.Common.csproj", "src/Services/WenDa/Sxb.WenDa.Common/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Shared/Sxb.Infrastructure.Core/Sxb.Infrastructure.Core.csproj", "src/Shared/Sxb.Infrastructure.Core/"]
COPY ["src/Services/WenDa/Sxb.WenDa.Infrastructure/Sxb.WenDa.Infrastructure.csproj", "src/Services/WenDa/Sxb.WenDa.Infrastructure/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/Sxb.Framework.Cache.RedisProfiler.csproj", "src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/"]
COPY ["src/Tools/Sxb.Framework.HttpContextLog/Sxb.Framework.HttpContextLog.csproj", "src/Tools/Sxb.Framework.HttpContextLog/"]
COPY ["src/Services/WenDa/Sxb.WenDa.Query.SQL/Sxb.WenDa.Query.SQL.csproj", "src/Services/WenDa/Sxb.WenDa.Query.SQL/"]
RUN dotnet restore "src/Services/WenDa/Sxb.WenDa.API/Sxb.WenDa.API.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/WenDa/Sxb.WenDa.API"
RUN dotnet build "Sxb.WenDa.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.WenDa.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.WenDa.API.dll"]