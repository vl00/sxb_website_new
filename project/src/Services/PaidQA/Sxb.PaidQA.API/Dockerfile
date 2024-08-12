#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/PaidQA/Sxb.PaidQA.API/Sxb.PaidQA.API.csproj", "src/Services/PaidQA/Sxb.PaidQA.API/"]
COPY ["src/Services/PaidQA/Sxb.PaidQA.Common/Sxb.PaidQA.Common.csproj", "src/Services/PaidQA/Sxb.PaidQA.Common/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Services/PaidQA/Sxb.PaidQA.Query.SQL/Sxb.PaidQA.Query.SQL.csproj", "src/Services/PaidQA/Sxb.PaidQA.Query.SQL/"]
RUN dotnet restore "src/Services/PaidQA/Sxb.PaidQA.API/Sxb.PaidQA.API.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/PaidQA/Sxb.PaidQA.API"
RUN dotnet build "Sxb.PaidQA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.PaidQA.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.PaidQA.API.dll"]