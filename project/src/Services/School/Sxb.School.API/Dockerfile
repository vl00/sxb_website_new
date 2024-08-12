#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/School/Sxb.School.API/Sxb.School.API.csproj", "src/Services/School/Sxb.School.API/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Services/School/Sxb.School.Infrastructure/Sxb.School.Infrastructure.csproj", "src/Services/School/Sxb.School.Infrastructure/"]
COPY ["src/Services/School/Sxb.School.Domain/Sxb.School.Domain.csproj", "src/Services/School/Sxb.School.Domain/"]
COPY ["src/Shared/Sxb.Domain.Abstractions/Sxb.Domain.Abstractions.csproj", "src/Shared/Sxb.Domain.Abstractions/"]
COPY ["src/Services/School/Sxb.School.Query.ES/Sxb.School.Query.ES.csproj", "src/Services/School/Sxb.School.Query.ES/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Shared/Sxb.Infrastructure.Core/Sxb.Infrastructure.Core.csproj", "src/Shared/Sxb.Infrastructure.Core/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/Sxb.Framework.Cache.RedisProfiler.csproj", "src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/"]
COPY ["src/Services/School/Sxb.School.Query.SQL/Sxb.School.Query.SQL.csproj", "src/Services/School/Sxb.School.Query.SQL/"]
COPY ["src/Services/School/Sxb.School.Common/Sxb.School.Common.csproj", "src/Services/School/Sxb.School.Common/"]
RUN dotnet restore "src/Services/School/Sxb.School.API/Sxb.School.API.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/School/Sxb.School.API"
RUN dotnet build "Sxb.School.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.School.API.csproj" -c Release -o /app/publish
#COPY ["Sxb.School.API.xml", "/app/publish/"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ="Asia/Shanghai"
ENTRYPOINT ["dotnet", "Sxb.School.API.dll"]