#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/WeWorkFinance/Sxb.WeWorkFinance.API/Sxb.WeWorkFinance.API.csproj", "src/Services/WeWorkFinance/Sxb.WeWorkFinance.API/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Services/WeWorkFinance/Sxb.WeWorkFinance.Infrastructure/Sxb.WeWorkFinance.Infrastructure.csproj", "src/Services/WeWorkFinance/Sxb.WeWorkFinance.Infrastructure/"]
COPY ["src/Services/WeWorkFinance/Sxb.WeWorkFinance.Domain/Sxb.WeWorkFinance.Domain.csproj", "src/Services/WeWorkFinance/Sxb.WeWorkFinance.Domain/"]
COPY ["src/Shared/Sxb.Domain.Abstractions/Sxb.Domain.Abstractions.csproj", "src/Shared/Sxb.Domain.Abstractions/"]
COPY ["src/Shared/Sxb.Infrastructure.Core/Sxb.Infrastructure.Core.csproj", "src/Shared/Sxb.Infrastructure.Core/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/Sxb.Framework.Cache.RedisProfiler.csproj", "src/Tools/Sxb.Framework.Cache.Redis/Sxb.Framework.Cache.RedisProfiler/"]
COPY ["src/Tools/Sxb.Framework.SearchAccessor/Sxb.Framework.SearchAccessor.csproj", "src/Tools/Sxb.Framework.SearchAccessor/"]
RUN dotnet restore "src/Services/WeWorkFinance/Sxb.WeWorkFinance.API/Sxb.WeWorkFinance.API.csproj"
COPY . .
WORKDIR "/src/src/Services/WeWorkFinance/Sxb.WeWorkFinance.API"
RUN dotnet build "Sxb.WeWorkFinance.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.WeWorkFinance.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.WeWorkFinance.API.dll"]