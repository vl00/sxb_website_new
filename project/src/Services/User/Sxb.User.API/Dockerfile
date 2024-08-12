#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/User/Sxb.User.API/Sxb.User.API.csproj", "src/Services/User/Sxb.User.API/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/Sxb.Framework.Cache.RedisProfiler.csproj", "src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/"]
COPY ["src/Services/User/Sxb.User.Query.SQL/Sxb.User.Query.SQL.csproj", "src/Services/User/Sxb.User.Query.SQL/"]
COPY ["src/Services/User/Sxb.User.Common/Sxb.User.Common.csproj", "src/Services/User/Sxb.User.Common/"]
COPY ["src/Services/User/Sxb.User.Infrastructure/Sxb.User.Infrastructure.csproj", "src/Services/User/Sxb.User.Infrastructure/"]
COPY ["src/Shared/Sxb.Infrastructure.Core/Sxb.Infrastructure.Core.csproj", "src/Shared/Sxb.Infrastructure.Core/"]
COPY ["src/Shared/Sxb.Domain.Abstractions/Sxb.Domain.Abstractions.csproj", "src/Shared/Sxb.Domain.Abstractions/"]
COPY ["src/Services/User/Sxb.User.Domain/Sxb.User.Domain.csproj", "src/Services/User/Sxb.User.Domain/"]
RUN dotnet restore "src/Services/User/Sxb.User.API/Sxb.User.API.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/User/Sxb.User.API"
RUN dotnet build "Sxb.User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.User.API.dll"]