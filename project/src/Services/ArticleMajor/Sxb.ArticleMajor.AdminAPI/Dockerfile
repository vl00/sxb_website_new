#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/ArticleMajor/Sxb.ArticleMajor.AdminAPI/Sxb.ArticleMajor.AdminAPI.csproj", "src/Services/ArticleMajor/Sxb.ArticleMajor.AdminAPI/"]
COPY ["src/Services/ArticleMajor/Sxb.ArticleMajor.Query.SQL/Sxb.ArticleMajor.Query.SQL.csproj", "src/Services/ArticleMajor/Sxb.ArticleMajor.Query.SQL/"]
COPY ["src/Services/ArticleMajor/Sxb.ArticleMajor.Common/Sxb.ArticleMajor.Common.csproj", "src/Services/ArticleMajor/Sxb.ArticleMajor.Common/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/Sxb.Framework.Cache.RedisProfiler.csproj", "src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/"]
RUN dotnet restore "src/Services/ArticleMajor/Sxb.ArticleMajor.AdminAPI/Sxb.ArticleMajor.AdminAPI.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/ArticleMajor/Sxb.ArticleMajor.AdminAPI"
RUN dotnet build "Sxb.ArticleMajor.AdminAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.ArticleMajor.AdminAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ="Asia/Shanghai"
ENTRYPOINT ["dotnet", "Sxb.ArticleMajor.AdminAPI.dll"]